using UnityEngine;

namespace Vision_Controller
{
    public class SphericalVision : AbstractVision
    {
        private Transform _obj;

        
        
        
        
        
        public SphericalVision(VisionController visionController) : base(visionController) { }

        
        
        public override bool CheckVisionArea(Vector3 relativePos)
        {
            bool result = false;


            _ = Physics.OverlapSphereNonAlloc(relativePos, GetMaxRadius, GetColliders, GetTargetLayer);

            for (int i = 0; i < GetMaxObjDetection; i++)
            {
                if (GetColliders[i] is null) continue;
                
                _obj = GetColliders[i].transform;
                if (CheckInside(_obj.position, relativePos))
                {
                    if (GetNotifyObjExit && !IsDetectedObjExist(_obj))
                    {
                        GetDetectedObjs.Add(_obj);
                        GetObjDetectedEvent?.Invoke(_obj);
                    }
                    else
                    {
                        GetObjDetectedEvent?.Invoke(_obj);
                    }
                    
                    result = true;
                }
                
                GetColliders[i] = null;
            }
            
            if (GetNotifyObjExit && GetDetectedObjs.Count > 0) ManageDetectedObjs(relativePos);
            
            return result;
        }
        
        private bool CheckInside(Vector3 objPos, Vector3 relativePos)
        {
            Vector3 targetDir = objPos - relativePos;

            float distance = targetDir.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;

            if (!GetBlockCheck) return true;
            
            return !CheckBlocked(targetDir, relativePos, _obj);
        }
        
        
        private void ManageDetectedObjs(Vector3 relativePos)
        {
            for (int i = 0; i < GetDetectedObjs.Count;)
            {
                _obj = GetDetectedObjs[i];
                if (CheckInside(_obj.position, relativePos))
                {
                    i++;
                    continue;
                }

                GetObjExitEvent?.Invoke(_obj);
                GetDetectedObjs.Remove(_obj);
            }
        }
    }
}