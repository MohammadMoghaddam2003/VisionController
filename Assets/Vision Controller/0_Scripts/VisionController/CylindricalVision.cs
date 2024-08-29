using UnityEngine;


namespace Vision_Controller
{
    public class CylindricalVision : AbstractVision
    {
        private readonly float _minHeight;
        private readonly float _maxHeight;
        private readonly int _fov;
        private Transform _obj;


        public CylindricalVision(VisionController visionController) : base(visionController)
        {
            _minHeight = visionController.GetMinHeight;
            _maxHeight = visionController.GetMaxHeight;
            _fov = visionController.GetFov;
        }


        public override bool CheckVisionArea(Vector3 relativePos)
        {
            bool result = false;
            
            Vector3 point0 = relativePos;
            point0.y = _minHeight + GetCenter.y;

            Vector3 point1 = relativePos;
            point1.y = _maxHeight + GetCenter.y;

            _ = Physics.OverlapCapsuleNonAlloc(point0, point1, GetMaxRadius, GetColliders, GetTargetLayer);

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
            if (targetDir.y < _minHeight || targetDir.y > _maxHeight) return false;

            Vector3 flatPos = new Vector3(targetDir.x, 0, targetDir.z);
            float distance = flatPos.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;
            
            Vector3 fovDir = MathHelper.Ang2Vec3(GetDirection);
            if(Vector3.Angle(fovDir, GetTransform.InverseTransformVector(flatPos)) > _fov * .5f) return false;

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