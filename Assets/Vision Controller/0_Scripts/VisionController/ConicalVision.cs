using UnityEngine;

namespace Vision_Controller
{
    public class ConicalVision : AbstractVision
    {
        private readonly int _fov;
        private Transform _obj;




        public ConicalVision(VisionController visionController) : base(visionController)
        {
            _fov = visionController.GetFov;
        }
        

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
                    if (GetNotifyObjExit && !IsObjExist(_obj))
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
            
            Vector3 fovDir = MathHelper.Ang2Vec3(GetDirection);
            if(Vector3.Angle(fovDir, GetTransform.InverseTransformVector(targetDir)) > _fov * .5f) return false;

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