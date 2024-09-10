using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Vision_Controller
{
    public class ConicalVision : AbstractVision
    {
        private readonly int _fov;
        private readonly int _fos;
        private Transform _obj;




        public ConicalVision(VisionController visionController) : base(visionController)
        {
            _fov = visionController.GetFov;
            _fos = visionController.GetSenseField;
        }
        

        public override void CheckVisionArea(Vector3 relativePos, out bool isSeen, out bool isSensed)
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
                    if (GetNotifyObjExit && !IsDetectedObjExist(_obj))
                    {
                        GetDetectedObjs.Add(_obj);
                        GetObjDetectedEvent?.Invoke(_obj);
                    }
                    else
                    {
                        GetObjDetectedEvent?.Invoke(_obj);
                    }

                    isSeen = true;
                }
                else if(GetCalculateSense && CheckInside(_obj.position, relativePos, _fos))
                {
                    if (GetNotifySensedObjExit && !IsSensedObjExist(_obj))
                    {
                        GetSensedObjs.Add(_obj);
                        GetObjSensedEvent?.Invoke(_obj);
                    }
                    else
                    {
                        GetObjSensedEvent?.Invoke(_obj);
                    }
                    
                    isSensed = true;
                }
                
                GetColliders[i] = null;
            }
            
            if (GetNotifyObjExit && GetDetectedObjs.Count > 0) 
                ManageObjs(relativePos, GetDetectedObjs, GetObjExitEvent, _fov, GetBlockCheck);
            
            if (GetCalculateSense && GetNotifySensedObjExit && GetSensedObjs.Count > 0) 
                ManageObjs(relativePos, GetSensedObjs, GetSensedObjExitEvent, _fos);
        }
        

        private bool CheckInside(Vector3 objPos, Vector3 relativePos, float area, bool checkBlocked = true)
        {
            Vector3 targetDir = objPos - relativePos;

            float distance = targetDir.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;
            
            Vector3 fovDir = MathHelper.Ang2Vec3(GetDirection);
            if(Vector3.Angle(fovDir, GetTransform.InverseTransformVector(targetDir)) > area * .5f) return false;
            
            if (!checkBlocked) return true;

            return !CheckBlocked(targetDir, relativePos, _obj);
        }


        private void ManageObjs(Vector3 relativePos, List<Transform> objsList, UnityEvent<Transform> exitEvent,
            float area, bool blockedCheck = true)
        {
            for (int i = 0; i < objsList.Count;)
            {
                _obj = objsList[i];
                if (CheckInside(_obj.position, relativePos, area, blockedCheck))
                {
                    i++;
                    continue;
                }

                exitEvent?.Invoke(_obj);
                objsList.Remove(_obj);
            }
        }
    }
}