using UnityEngine;


namespace Vision_Controller
{
    public class CylindricalVision : Vision
    {
        private Transform _obj;

        
        
        #region Methods

        

        public CylindricalVision(Transform trans, VisionData data) : base(trans, data) { }


        public override void ManageArea(Vector3 relativePos, out bool isSeen, out bool isSensed)
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

                if (CheckInside(_obj, relativePos, GetFov, GetBlockCheck))
                {
                    ObjectSeen(_obj);
                    isSeen = true;
                }
                else if (GetCalculateSense && CheckInside(_obj, relativePos, GetFos, true))
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
            Vector3 targetDir = obj.position - relativePos;
            if (targetDir.y < GetMinHeight || targetDir.y > GetMaxHeight) return false;

            Vector3 flatPos = new Vector3(targetDir.x, 0, targetDir.z);
            float distance = flatPos.magnitude;
            if (distance < GetMinRadius || distance > GetMaxRadius) return false;

            Vector3 fovDir = MathHelper.Ang2Vec3(GetDirection);
            if (Vector3.Angle(fovDir, GetTransform.InverseTransformVector(flatPos)) > areaAngle * .5f) return false;

            if (!checkBlocked) return true;

            return !CheckBlocked(targetDir, relativePos, obj);
        }
        
    
        
        

#if UNITY_EDITOR

        public override void DrawArea(Vector3 visionRelativePos, int areaAngle, float projection)
        {
            Quaternion rotation = GetTransform.rotation;
            Vector3 pos = visionRelativePos;


            pos.y = GetVisionData.GetMinHeight + visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, GetVisionData.GetDirection, rotation);
            Draw(areaAngle, projection);

            pos.y = GetVisionData.GetMaxHeight + visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, GetVisionData.GetDirection, rotation);
            Draw(areaAngle, projection, true);
        }
        
#endif

        
        
        #endregion
    }
}