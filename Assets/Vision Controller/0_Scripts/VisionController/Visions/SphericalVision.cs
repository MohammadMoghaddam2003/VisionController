using UnityEngine;

namespace Vision_Controller
{
    public class SphericalVision : Vision
    {
        private Transform _obj;



        #region Methods

        

        
        public SphericalVision(Transform trans, VisionData data) : base(trans, data) { }

        
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
                if (CheckInside(_obj, relativePos))
                {
                    ObjectSeen(_obj);
                    isSeen = true;
                }
                
                GetColliders[i] = null;
            }
            
            if (GetNotifyDetectedObjExit && GetDetectedObjs.Count > 0) 
                TrackDetectedObjs(relativePos, CheckInside);
        }


        protected override bool CheckInside(Transform obj, Vector3 relativePos, float areaAngle = 0, bool checkBlocked = false)
        {
            if (!_obj) return false;
            
            Vector3 targetDir = obj.position - relativePos;

            float distance = targetDir.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;

            if (!GetBlockCheck) return true;
            
            return !CheckBlocked(targetDir, relativePos, obj);
        }

        
        
        

        


#if UNITY_EDITOR
        
        public override void DrawArea(Vector3 visionRelativePos, int areaAngle, float projection)
        {
            Gizmos.DrawWireSphere(visionRelativePos, GetVisionData.GetMinRadius);
            Gizmos.DrawWireSphere(visionRelativePos, GetVisionData.GetMaxRadius);
        }
        
#endif   

        
        
        #endregion
    }
}