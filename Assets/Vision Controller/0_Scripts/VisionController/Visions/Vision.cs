using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace Vision_Controller
{
    public abstract class Vision
    {
        #region Variables

        

        protected VisionData GetVisionData { get; private set; }
        protected Transform GetTransform { get; private set; }
        protected Collider[] GetColliders { get; private set; }
        protected List<Transform> GetDetectedObjs { get; private set; }
        protected List<Transform> GetSensedObjs { get; private set; }
        protected LayerMask GetTargetLayer { get; private set; }
        protected Vector3 GetCenter { get; private set; }
        protected int GetDirection { get; private set; }
        protected int GetMaxObjDetection { get; private set; }
        
        // Field of view
        protected int GetFov { get; private set; }
        
        // Field of sense
        protected int GetFos { get; private set; }
        
        protected float GetMinRadius { get; private set; }
        protected float GetMaxRadius { get; private set; }
        protected float GetMinHeight { get; private set; }
        protected float GetMaxHeight { get; private set; }
        protected bool GetNotifyDetectedObjExit { get; private set; }
        protected bool GetBlockCheck { get; private set; }
        protected bool GetCalculateSense { get; private set; }
        protected bool GetNotifySensedObjExit { get; private set; }



        private readonly LayerMask _obstaclesLayer;
        private UnityEvent<Transform> GetObjDetectedEvent { get; set; }
        private UnityEvent<Transform> GetDetectedObjExitEvent { get; set; }
        private UnityEvent<Transform> GetObjSensedEvent { get; set; }
        private UnityEvent<Transform> GetSensedObjExitEvent { get; set; }



        #endregion




        #region Methods

        


        protected Vision(Transform trans, VisionData data)
        {
            GetTransform = trans;
            GetVisionData = data;
            GetTargetLayer = data.GetTargetLayer;
            GetCenter = data.GetCenter;
            GetDirection = data.GetDirection;
            GetMaxObjDetection = data.GetMaxObjDetection;
            GetFov = data.GetFov;
            GetFos = data.GetFos;
            GetMinRadius = data.GetMinRadius;
            GetMaxRadius = data.GetMaxRadius;
            GetMinHeight = data.GetMinHeight;
            GetMaxHeight = data.GetMaxHeight;
            GetNotifyDetectedObjExit = data.GetNotifyDetectedObjExit;
            GetBlockCheck = data.GetBlockCheck;
            GetCalculateSense = data.GetCalculateSense;
            GetNotifySensedObjExit = data.GetNotifySensedObjExit;
            GetObjDetectedEvent = data.onObjDetected;
            GetDetectedObjExitEvent = data.onDetectedObjExit;
            GetObjSensedEvent = data.onObjSensed;
            GetSensedObjExitEvent = data.onSensedObjExit;
            _obstaclesLayer = data.GetObstaclesLayer;



            GetColliders = new Collider[GetMaxObjDetection];
            
            if(GetNotifyDetectedObjExit) GetDetectedObjs = new List<Transform>();
            
            if(!GetCalculateSense || !GetNotifySensedObjExit) return;
            GetSensedObjs = new List<Transform>();
        }


        
        /// <summary>
        /// It checks whether the target is behind something and is blocked!
        /// </summary>
        /// <param name="targetDir"> The direction of the target </param>
        /// <param name="relativePos">
        /// The position of the vision based on the object's position and the center field in the inspector
        /// </param>
        /// <param name="target"> The detected object </param>
        protected bool CheckBlocked(Vector3 targetDir, Vector3 relativePos, Transform target)
        {
            Ray ray = new Ray(relativePos, targetDir);
            
            if (!Physics.Raycast(ray, out RaycastHit hit, GetMaxRadius + GetMinRadius, _obstaclesLayer + GetTargetLayer))
                return true;
            
            return hit.transform != target;
        }


        protected void ObjectSeen(Transform obj)
        {
            if (IsObjExist(obj, GetDetectedObjs)) return;
            
            
            if (GetNotifyDetectedObjExit)
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
            if(IsObjExist(obj, GetSensedObjs)) return;

            if (GetNotifySensedObjExit)
            {
                GetSensedObjs.Add(obj);
                GetObjSensedEvent?.Invoke(obj);
            }
            else
            {
                GetObjSensedEvent?.Invoke(obj);
            }
        }
        

        
        /// <summary>
        /// It checks whether the object is on the list!
        /// </summary>
        private static bool IsObjExist(Transform obj, List<Transform> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == obj) return true;
            }

            return false;
        }


        
        
        protected delegate bool CheckInsideMethod(Transform obj, Vector3 relativePos, float areaAngle, bool checkBlocked);



        /// <summary>
        /// Tracks the detected objects and informs when one of them goes outside of the vision/sense area!
        /// </summary>
        /// <param name="relativePos">
        /// The position of the vision based on the object's position and the center field in the inspector
        /// </param>
        protected void TrackDetectedObjs(Vector3 relativePos, CheckInsideMethod checkInside)
        {
            Transform obj;
            for (int i = 0; i < GetDetectedObjs.Count;)
            {
                obj = GetDetectedObjs[i];

                if (checkInside.Invoke(obj, relativePos, GetFov, GetBlockCheck))
                {
                    i++;
                    continue;
                }

                GetDetectedObjExitEvent?.Invoke(obj);
                RemoveObj(obj, GetDetectedObjs);
            }
        }
        
        
        
        /// <summary>
        /// Tracks the detected objects and informs when one of them goes outside of the vision/sense area!
        /// </summary>
        /// <param name="relativePos">
        /// The position of the vision based on the object's position and the center field in the inspector
        /// </param>
        protected void TrackSensedObjs(Vector3 relativePos, CheckInsideMethod checkInside)
        {
            Transform obj;
            
            for (int i = 0; i < GetSensedObjs.Count;)
            {
                obj = GetSensedObjs[i];
                
                if(!checkInside.Invoke(obj, relativePos, GetFov, GetBlockCheck))
                {
                    if (checkInside.Invoke(obj, relativePos, GetFos, true))
                    {
                        i++;
                        continue;
                    }   
                }

                GetSensedObjExitEvent?.Invoke(obj);
                RemoveObj(obj, GetSensedObjs);
            }
        }


        private static void RemoveObj(Transform obj, List<Transform> list) => list.Remove(obj);
        


        
        /// <summary>
        /// Manages the vision/sense area and informs when any object is detected!
        /// </summary>
        /// <param name="relativePos">
        /// The position of the vision based on the object's position and the center field in the inspector
        /// </param>
        /// <param name="isSeen"> It will become true when any object goes inside the vision area </param>
        /// <param name="isSensed"> It will become true when any object goes inside the sense area </param>
        public abstract void ManageArea(Vector3 relativePos, out bool isSeen, out bool isSensed);


        
        /// <summary>
        /// It checks whether the object is inside the vision/sense area!
        /// </summary>
        /// <param name="obj"> The object that must be checked </param>
        /// <param name="relativePos">
        /// The position of the vision based on the object's position and the center field in the inspector
        /// </param>
        /// <param name="areaAngle"> The angle of the vision/sense area </param>
        /// <param name="checkBlocked"> Does it check if the object is blocked or not? </param>
        protected abstract bool CheckInside(Transform obj, Vector3 relativePos, float areaAngle, bool checkBlocked);
        
        
        
        
        
        
#if UNITY_EDITOR
        
        
        
        
        /// <summary>
        /// Changes the matrix of the Gizmos and Handles!
        /// </summary>
        /// <param name="pos"> The position of matrix </param>
        /// <param name="axis"> The axis that the matrix should turn around </param>
        /// <param name="degree"> The amount of rotation of the matrix by degree </param>
        /// <param name="rotation"> The base/current rotation </param>
        protected static void ConfigureMatrices(Vector3 pos, Vector3 axis, float degree, Quaternion rotation) => 
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(pos, axis, (-degree + 90), rotation);


        
        /// <summary>
        /// Draws the vision/sense area!
        /// </summary>
        protected void Draw(int areaAngle, float projection, bool connectVertices = false, bool drawDisk = false)
        {
            if(areaAngle == GetVisionData.GetFos && GetVisionData.GetFos <= GetVisionData.GetFov) return;
            
            if(areaAngle is 0) return;
            
            float x = MathHelper.Pythagoras_GetSide(1, projection);
            
            Vector3 minRight = new Vector3(x, 0, projection) * GetVisionData.GetMinRadius;
            Vector3 minLeft = new Vector3(-x, 0, projection) * GetVisionData.GetMinRadius;
            Vector3 maxRight = new Vector3(x, 0, projection) * GetVisionData.GetMaxRadius;
            Vector3 maxLeft = new Vector3(-x, 0, projection) * GetVisionData.GetMaxRadius;

            Handles.DrawWireArc(default, Vector3.up, minLeft, areaAngle,  GetVisionData.GetMinRadius);
            Handles.DrawWireArc(default, Vector3.up, maxLeft, areaAngle, GetVisionData.GetMaxRadius);
            
            if(areaAngle is 360) return;

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
                if(areaAngle is 0 or 360) return;

                Vector3 diskCenter = new Vector3(0, 0, projection);
            
                Handles.DrawWireDisc(diskCenter *  GetVisionData.GetMinRadius, Vector3.forward,
                    x * GetVisionData.GetMinRadius);
                Handles.DrawWireDisc(diskCenter *  GetVisionData.GetMaxRadius, Vector3.forward,
                    x * GetVisionData.GetMaxRadius);
            }
        }


        
        /// <summary>
        /// It is an interface method that should be implemented in each vision mode to correctly draw the vision/sense
        /// area of each vision mode!
        /// </summary>
        public abstract void DrawArea(Vector3 visionRelativePos, int areaAngle, float projection);
   
        
        
#endif
        
        
        
        #endregion
    }
}