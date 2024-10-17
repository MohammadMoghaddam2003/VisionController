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
                
                _obj = GetColliders[i].transform;
                if (CheckInside(_obj.position, relativePos))
                {
                    ObjectSeen(_obj);
                    isSeen = true;
                }
                
                GetColliders[i] = null;
            }
            
            if (GetNotifyDetectedObjExit && GetDetectedObjs.Count > 0) 
                TrackObjs(relativePos, GetDetectedObjs, GetObjExitEvent, 0, false, CheckInside);
        }


        protected override bool CheckInside(Vector3 objPos, Vector3 relativePos, float areaAngle = 0, bool checkBlocked = false)
        {
            Vector3 targetDir = objPos - relativePos;

            float distance = targetDir.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;

            if (!GetBlockCheck) return true;
            
            return !CheckBlocked(targetDir, relativePos, _obj);
        }

        
        
        

        


#if UNITY_EDITOR
        
        public override void DrawArea(Vector3 visionRelativePos, int area, float projection)
        {
            Gizmos.DrawWireSphere(visionRelativePos, GetVisionData.GetMinRadius);
            Gizmos.DrawWireSphere(visionRelativePos, GetVisionData.GetMaxRadius);
        }
        
#endif   

        
        
        #endregion
    }
}