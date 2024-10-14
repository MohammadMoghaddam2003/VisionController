using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace Vision_Controller
{
    public abstract class Vision
    {
        protected VisionData GetVisionData { get; private set; }
        protected Transform GetTransform { get; private set; }
        protected Collider[] GetColliders { get; private set; }
        protected List<Transform> GetDetectedObjs { get; private set; }
        protected List<Transform> GetSensedObjs { get; private set; }
        protected UnityEvent<Transform> GetObjExitEvent { get; private set; }
        protected UnityEvent<Transform> GetSensedObjExitEvent { get; private set; }
        protected LayerMask GetTargetLayer { get; private set; }
        protected Vector3 GetCenter { get; private set; }
        protected int GetDirection { get; private set; }
        protected int GetMaxObjDetection { get; private set; }
        protected int GetFov { get; private set; }
        protected int GetFos { get; private set; }
        protected float GetMinRadius { get; private set; }
        protected float GetMaxRadius { get; private set; }
        protected float GetMinHeight { get; private set; }
        protected float GetMaxHeight { get; private set; }
        protected bool GetNotifyObjExit { get; private set; }
        protected bool GetBlockCheck { get; private set; }
        protected bool GetCalculateSense { get; private set; }
        protected bool GetNotifySensedObjExit { get; private set; }



        private readonly LayerMask _obstaclesLayer;
        private UnityEvent<Transform> GetObjDetectedEvent { get; set; }
        private UnityEvent<Transform> GetObjSensedEvent { get; set; }



        
        
        

        protected Vision(Transform trans, VisionData data)
        {
            GetVisionData = data;
            GetTransform = trans;
            GetTargetLayer = data.GetTargetLayer;
            _obstaclesLayer = data.GetObstaclesLayer;
            GetCenter = data.GetCenter;
            GetDirection = data.GetDirection;
            GetMaxObjDetection = data.GetMaxObjDetection;
            GetFov = data.GetFov;
            GetFos = data.GetSenseField;
            GetMinRadius = data.GetMinRadius;
            GetMaxRadius = data.GetMaxRadius;
            GetMinHeight = data.GetMinHeight;
            GetMaxHeight = data.GetMaxHeight;
            GetNotifyObjExit = data.GetNotifyObjExit;
            GetBlockCheck = data.GetBlockCheck;
            GetCalculateSense = data.GetCalculateSense;
            GetNotifySensedObjExit = data.GetNotifySensedObjExit;
            GetObjDetectedEvent = data.onObjDetected;
            GetObjExitEvent = data.onObjExit;
            GetObjSensedEvent = data.onObjSensed;
            GetSensedObjExitEvent = data.onSensedObjExit;
            
            

            GetColliders = new Collider[GetMaxObjDetection];
            
            if(GetNotifyObjExit) GetDetectedObjs = new List<Transform>();
            
            if(!GetCalculateSense || !GetNotifySensedObjExit) return;
            GetSensedObjs = new List<Transform>();
        }


        
        protected bool CheckBlocked(Vector3 targetDir, Vector3 relativePos, Transform obj)
        {
            Ray ray = new Ray(relativePos, targetDir);
            
            if (!Physics.Raycast(ray, out RaycastHit hit, GetMaxRadius, _obstaclesLayer + GetTargetLayer))
                return true;
            
            return hit.transform != obj;
        }


        protected void ObjectSeen(Transform obj)
        {
            if (GetNotifyObjExit && !IsObjExist(obj, GetDetectedObjs))
            {
                GetDetectedObjs.Add(obj);
                GetObjDetectedEvent?.Invoke(obj);
            }
            else
            {
                GetObjDetectedEvent?.Invoke(obj);
            }
        }
        
        
        protected void ObjectSensed(Transform obj)
        {
            if (GetNotifySensedObjExit && !IsObjExist(obj, GetSensedObjs))
            {
                GetSensedObjs.Add(obj);
                GetObjSensedEvent?.Invoke(obj);
            }
            else
            {
                GetObjSensedEvent?.Invoke(obj);
            }
        }

        private static bool IsObjExist(Transform obj, List<Transform> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == obj) return true;
            }

            return false;
        }


        protected delegate bool CheckInsideMethod(Vector3 objPos, Vector3 relativePos, float area, bool checkBlocked);


        protected void ManageObjs(Vector3 relativePos, List<Transform> objsList, UnityEvent<Transform> exitEvent,
            float area, bool blockedCheck, CheckInsideMethod checkInside)
        {
            Transform obj;
            
            for (int i = 0; i < objsList.Count;)
            {
                obj = objsList[i];
                if (checkInside.Invoke(obj.position, relativePos, area, blockedCheck))
                {
                    i++;
                    continue;
                }

                exitEvent?.Invoke(obj);
                objsList.Remove(obj);
            }
        }


        public abstract void ManageArea(Vector3 relativePos, out bool isSeen, out bool isSensed);
        
        
        
        
#if UNITY_EDITOR
        
        protected static void ConfigureMatrices(Vector3 pos, Vector3 axis, float degree, Quaternion rotation) => 
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(pos, axis, (-degree + 90), rotation);


        protected void Draw(int area, float projection, bool connectVertices = false, bool drawDisk = false)
        {
            if(area == GetVisionData.GetSenseField && GetVisionData.GetSenseField <= GetVisionData.GetFov) return;
            
            if(area is 0) return;
            
            float x = MathHelper.Pythagoras_UnknownSide(1, projection);
            
            Vector3 minRight = new Vector3(x, 0, projection) *  GetVisionData.GetMinRadius;
            Vector3 minLeft = new Vector3(-x, 0, projection) *  GetVisionData.GetMinRadius;
            Vector3 maxRight = new Vector3(x, 0, projection) * GetVisionData.GetMaxRadius;
            Vector3 maxLeft = new Vector3(-x, 0, projection) * GetVisionData.GetMaxRadius;

            Handles.DrawWireArc(default, Vector3.up, minLeft, area,  GetVisionData.GetMinRadius);
            Handles.DrawWireArc(default, Vector3.up, maxLeft, area, GetVisionData.GetMaxRadius);
            
            if(area is 360) return;

            Gizmos.DrawLine(minRight, maxRight);
            Gizmos.DrawLine(minLeft, maxLeft);

            if (!connectVertices)
            {
                if(!drawDisk) return;
                
                DrawDisk();
                return;
            }

            float height = -(GetVisionData.GetMaxHeight - GetVisionData.GetMinHeight);
            Gizmos.DrawLine(minRight, new Vector3(minRight.x, height, minRight.z));
            Gizmos.DrawLine(minLeft, new Vector3(minLeft.x, height, minLeft.z));
            Gizmos.DrawLine(maxRight, new Vector3(maxRight.x, height, maxRight.z));
            Gizmos.DrawLine(maxLeft, new Vector3(maxLeft.x, height, maxLeft.z));

            void DrawDisk()
            {
                if(area is 0 or 360) return;

                Vector3 diskCenter = new Vector3(0, 0, projection);
            
                Handles.DrawWireDisc(diskCenter *  GetVisionData.GetMinRadius, Vector3.forward,
                    x * GetVisionData.GetMinRadius);
                Handles.DrawWireDisc(diskCenter *  GetVisionData.GetMaxRadius, Vector3.forward,
                    x * GetVisionData.GetMaxRadius);
            }
        }


        public abstract void DrawArea(Vector3 visionRelativePos, int area, float projection);
        
#endif
    }
}