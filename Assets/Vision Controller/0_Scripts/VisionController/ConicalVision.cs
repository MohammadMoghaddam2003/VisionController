using UnityEditor;
using UnityEngine;


namespace Vision_Controller
{
    public class ConicalVision : Vision
    {
        private readonly int _fov;
        private readonly int _fos;
        private Transform _obj;




        public ConicalVision(Transform trans, VisionData data) : base(trans, data)
        {
            _fov = data.GetFov;
            _fos = data.GetSenseField;
        }
        

        public override void ManageArea(Vector3 relativePos, out bool isSeen, out bool isSensed)
        {
            isSeen = false;
            isSensed = false;
            
            _ = Physics.OverlapSphereNonAlloc(relativePos, GetMaxRadius, GetColliders, GetTargetLayer);

            for (int i = 0; i < GetMaxObjDetection; i++)
            {
                if (GetColliders[i] is null) continue;
                
                _obj = GetColliders[i].transform;
                
                if (CheckInside(_obj.position, relativePos, _fov, GetBlockCheck))
                {
                    ObjectSeen(_obj);
                    isSeen = true;
                }
                else if(GetCalculateSense && CheckInside(_obj.position, relativePos, _fos, true))
                {
                    ObjectSensed(_obj);
                    isSensed = true;
                }
                
                GetColliders[i] = null;
            }
            
            
            if (GetNotifyObjExit && GetDetectedObjs.Count > 0) 
                ManageObjs(relativePos, GetDetectedObjs, GetObjExitEvent, _fov, GetBlockCheck, CheckInside);
            
            if (GetCalculateSense && GetNotifySensedObjExit && GetSensedObjs.Count > 0) 
                ManageObjs(relativePos, GetSensedObjs, GetSensedObjExitEvent, _fos, true, CheckInside);
        }
        

        private bool CheckInside(Vector3 objPos, Vector3 relativePos, float area, bool checkBlocked)
        {
            Vector3 targetDir = objPos - relativePos;

            float distance = targetDir.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;
            
            Vector3 fovDir = MathHelper.Ang2Vec3(GetDirection);
            if(Vector3.Angle(fovDir, GetTransform.InverseTransformVector(targetDir)) > area * .5f) return false;
            
            if (!checkBlocked) return true;

            return !CheckBlocked(targetDir, relativePos, _obj);
        }


#if UNITY_EDITOR
        
        public override void DrawArea(Vector3 visionRelativePos, int area, float projection)
        {
            Quaternion rotation = GetTransform.rotation;
            

            ConfigureMatrices(visionRelativePos, Vector3.up, GetVisionData.GetDirection, rotation);
            Draw(area, projection);

            rotation *= Quaternion.AngleAxis((-GetVisionData.GetDirection + 90), Vector3.up);
            
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(visionRelativePos, Vector3.forward, 90, rotation);
            Draw(area, projection, false, true);
        }
    }
    
#endif
}