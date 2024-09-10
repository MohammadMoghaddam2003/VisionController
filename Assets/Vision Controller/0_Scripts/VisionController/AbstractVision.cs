using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Vision_Controller
{
    public abstract class AbstractVision
    {
        protected Transform GetTransform { get; private set; }
        protected Collider[] GetColliders { get; private set; }
        protected List<Transform> GetDetectedObjs { get; private set; }
        protected List<Transform> GetSensedObjs { get; private set; }
        protected UnityEvent<Transform> GetObjDetectedEvent { get; private set; }
        protected UnityEvent<Transform> GetObjExitEvent { get; private set; }
        protected UnityEvent<Transform> GetObjSensedEvent { get; private set; }
        protected UnityEvent<Transform> GetSensedObjExitEvent { get; private set; }
        protected LayerMask GetTargetLayer { get; private set; }
        protected Vector3 GetCenter { get; private set; }
        protected int GetDirection { get; private set; }
        protected int GetMaxObjDetection { get; private set; }
        protected float GetMinRadius { get; private set; }
        protected float GetMaxRadius { get; private set; }
        protected bool GetNotifyObjExit { get; private set; }
        protected bool GetBlockCheck { get; private set; }
        protected bool GetCalculateSense { get; private set; }
        protected bool GetNotifySensedObjExit { get; private set; }



        private readonly LayerMask _obstaclesLayer;

        
        

        protected AbstractVision(VisionController visionController)
        {
            GetTransform = visionController.transform;
            GetTargetLayer = visionController.GetTargetLayer;
            _obstaclesLayer = visionController.GetObstaclesLayer;
            GetCenter = visionController.GetCenter;
            GetDirection = visionController.GetDirection;
            GetMaxObjDetection = visionController.GetMaxObjDetection;
            GetMinRadius = visionController.GetMinRadius;
            GetMaxRadius = visionController.GetMaxRadius;
            GetNotifyObjExit = visionController.GetNotifyObjExit;
            GetBlockCheck = visionController.GetBlockCheck;
            GetCalculateSense = visionController.GetCalculateSense;
            GetNotifySensedObjExit = visionController.GetNotifySensedObjExit;
            GetObjDetectedEvent = visionController.onObjDetected;
            GetObjExitEvent = visionController.onObjExit;
            GetObjSensedEvent = visionController.onObjSensed;
            GetSensedObjExitEvent = visionController.onSensedObjExit;

            GetColliders = new Collider[GetMaxObjDetection];
            
            if(GetNotifyObjExit) GetDetectedObjs = new List<Transform>();
            
            if(!GetCalculateSense || !GetNotifySensedObjExit) return;
            GetSensedObjs = new List<Transform>();
        }


        
        protected bool CheckBlocked(Vector3 targetDir, Vector3 relativePos, Transform obj)
        {
            Ray ray = new Ray(relativePos, targetDir);
            if (Physics.Raycast(ray, out RaycastHit hit, GetMaxRadius, _obstaclesLayer + GetTargetLayer))
            {
                if (hit.transform == obj) return false;
            }
            
            return true;
        }
        
        
        
        protected bool IsDetectedObjExist(Transform obj)
        {
            for (int i = 0; i < GetDetectedObjs.Count; i++)
            {
                if (GetDetectedObjs[i] == obj) return true;
            }

            return false;
        }
        
        protected bool IsSensedObjExist(Transform obj)
        {
            for (int i = 0; i < GetSensedObjs.Count; i++)
            {
                if (GetSensedObjs[i] == obj) return true;
            }

            return false;
        }
        
        
        
        
        public virtual void CheckVisionArea(Vector3 relativePos, out bool isSeen, out bool isSensed)
        {
            isSeen = false;
            isSensed = false;
        }
        
        public virtual bool CheckVisionArea(Vector3 relativePos)
        {
            return false;
        }
    }
}