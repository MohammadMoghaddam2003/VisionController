using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


namespace Vision_Controller
{
    public class CylindricalVision : Vision
    {
        private Transform _obj;


        public CylindricalVision(Transform trans, VisionData data) : base(trans, data) { }


        public override void CheckVisionArea(Vector3 relativePos, out bool isSeen, out bool isSensed)
        {
            isSeen = false;
            isSensed = false;
            
            
            Vector3 point0 = relativePos;
            point0.y = GetMinHeight + GetCenter.y;

            Vector3 point1 = relativePos;
            point1.y = GetMaxHeight + GetCenter.y;

            _ = Physics.OverlapCapsuleNonAlloc(point0, point1, GetMaxRadius, GetColliders, GetTargetLayer);

            for (int i = 0; i < GetMaxObjDetection; i++)
            {
                if (GetColliders[i] is null) continue;
                
                _obj = GetColliders[i].transform;
                if (CheckInside(_obj.position, relativePos, GetFov, GetBlockCheck))
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
                else if(GetCalculateSense && CheckInside(_obj.position, relativePos, GetFos))
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
                ManageObjs(relativePos, GetDetectedObjs, GetObjExitEvent, GetFov, GetBlockCheck);
            
            if (GetCalculateSense && GetNotifySensedObjExit && GetSensedObjs.Count > 0) 
                ManageObjs(relativePos, GetSensedObjs, GetSensedObjExitEvent, GetFos);
        }


        private bool CheckInside(Vector3 objPos, Vector3 relativePos, float area, bool checkBlocked = true)
        {
            Vector3 targetDir = objPos - relativePos;
            if (targetDir.y < GetMinHeight || targetDir.y > GetMaxHeight) return false;

            Vector3 flatPos = new Vector3(targetDir.x, 0, targetDir.z);
            float distance = flatPos.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;
            
            Vector3 fovDir = MathHelper.Ang2Vec3(GetDirection);
            if(Vector3.Angle(fovDir, GetTransform.InverseTransformVector(flatPos)) > area * .5f) return false;

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
        
        
#if UNITY_EDITOR        
        
        public override void DrawArea(Vector3 visionRelativePos, int area, float projection)
        {
            Quaternion rotation = GetTransform.rotation;
            Vector3 pos = visionRelativePos;
            

            pos.y = GetMinHeight + visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, GetVisionData.GetDirection, rotation);
            Draw(area, projection);
            
            pos.y = GetMaxHeight + visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, GetVisionData.GetDirection, rotation);
            Draw(area, projection,true);
        }
    }
    
    
#endif
}