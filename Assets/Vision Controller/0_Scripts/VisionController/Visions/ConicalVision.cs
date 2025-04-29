using UnityEditor;
using UnityEngine;


namespace Vision_Controller
{
    public class ConicalVision : Vision
    {
        #region Variables

        
        private readonly int _fov;
        private readonly int _fos;
        private Transform _obj;

        
        #endregion


        

        #region Methods

        
        
        public ConicalVision(Transform trans, VisionData data) : base(trans, data)
        {
            _fov = data.GetFov;
            _fos = data.GetFos;
        }
        

        public override void ManageArea(Vector3 relativePos, out bool isSeen, out bool isSensed)
        {
            isSeen = false;
            isSensed = false;
            
            _ = Physics.OverlapSphereNonAlloc(relativePos, GetMaxRadius, GetColliders, GetTargetLayer);

            for (int i = 0; i < GetMaxObjDetection; i++)
            {
                if (GetColliders[i] is null) continue;
                
                if (GetColliders[i].transform == GetTransform)
                {
                    GetColliders[i] = null;
                    continue;
                }
                
                _obj = GetColliders[i].transform;
                
                if (CheckInside(_obj, relativePos, _fov, GetBlockCheck))
                {
                    ObjectSeen(_obj);
                    isSeen = true;
                }
                else if(GetCalculateSense && CheckInside(_obj, relativePos, _fos, true))
                {
                    ObjectSensed(_obj);
                    isSensed = true;
                }
                
                GetColliders[i] = null;
            }
            
            
            if (GetNotifyDetectedObjExit && GetDetectedObjs.Count > 0) 
                TrackDetectedObjs(relativePos, CheckInside);
            
            if (GetCalculateSense && GetNotifySensedObjExit && GetSensedObjs.Count > 0) 
                TrackSensedObjs(relativePos, CheckInside);
        }


        protected override bool CheckInside(Transform obj, Vector3 relativePos, float areaAngle, bool checkBlocked)
        {
            if (!_obj) return false;
            
            Vector3 targetDir = obj.position - relativePos;

            float distance = targetDir.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;
            
            Vector3 fovDir = MathHelper.Ang2Vec3(GetDirection);
            if(Vector3.Angle(fovDir, GetTransform.InverseTransformVector(targetDir)) > areaAngle * .5f) return false;
            
            if (!checkBlocked) return true;

            return !CheckBlocked(targetDir, relativePos, obj);
        }

        
        
        

#if UNITY_EDITOR
        
        public override void DrawArea(Vector3 visionRelativePos, int areaAngle, float projection)
        {
            Quaternion rotation = GetTransform.rotation;
            

            ConfigureMatrices(visionRelativePos, Vector3.up, GetVisionData.GetDirection, rotation);
            Draw(areaAngle, projection);

            rotation *= Quaternion.AngleAxis((-GetVisionData.GetDirection + 90), Vector3.up);
            
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(visionRelativePos, Vector3.forward, 90, rotation);
            Draw(areaAngle, projection, false, true);
        }

#endif
        
        
        
        #endregion
    }
}