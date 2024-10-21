using UnityEngine;
using UnityEngine.Events;

namespace Vision_Controller
{
    [System.Serializable]
    public class VisionData
    {
        [SerializeField] private VisionFactory visionFactory;

        
        public VisionFactory GetVisionFactory => visionFactory;


        // The vision modes determine how to calculate the vision
        public VisionMode GetMode => visionFactory ? visionFactory.Mode() : VisionMode.CylindricalVision;

        
        // The layer of the objects which should be detected
        [SerializeField] private LayerMask targetLayer;
        public LayerMask GetTargetLayer => targetLayer;
        
        
        // The layer of the objects which can block the targets so they don't detect
        [SerializeField] private LayerMask obstaclesLayer;
        public LayerMask GetObstaclesLayer => obstaclesLayer;

        
        [Range(0, 360)]
        [SerializeField] private int direction = 90;
        public int GetDirection => direction;
        
        
        
        [SerializeField] private Vector3 center = Vector3.zero;
        public Vector3 GetCenter => center;

        
        
        
        //This specifies that every few seconds it should check if any objects is in the vision!
        [SerializeField] private float recheckTime = .2f;
        public float GetRecheckTime => recheckTime;



        [Range(0, 360)] [SerializeField] 
        // Field of sense
        private int fov = 60;
        public int GetFov => fov;
        
        
        [Range(0, 360)] [SerializeField]
        // Field of sense
        private int fos = 150;
        public int GetFos => fos;

        
        
        //Any object closer than the min radius isn't detected!
        [SerializeField] private float minRadius = .7f;
        public float GetMinRadius { get => minRadius; set => minRadius = value; }


        //Any object further away than the max radius isn't detected!
        [SerializeField] private float maxRadius = 5f;
        public float GetMaxRadius { get => maxRadius; set => maxRadius = value; }


        
        //Maximum objects that can be detected!
        [SerializeField] private int maxObjDetection = 10;
        public int GetMaxObjDetection => maxObjDetection;

        
        [SerializeField] private float minHeight = -.7f;
        public float GetMinHeight { get => minHeight; set => minHeight = value; }


        [SerializeField] private float maxHeight = 1.3f;
        public float GetMaxHeight { get => maxHeight; set => maxHeight = value; }

        
        
        // It will inform when a detected object goes outside the vision field
        [SerializeField] private bool notifyDetectedObjExit = true;
        public bool GetNotifyDetectedObjExit => notifyDetectedObjExit;
        
        
        // Determines whether to calculate whether the target is behind something and blocked
        [SerializeField] private bool blockCheck = true;
        public bool GetBlockCheck => blockCheck;
        
        
        [SerializeField] private bool calculateSense = true;
        public bool GetCalculateSense => calculateSense;
        
        
        // It will inform when a sensed object goes outside the sense field
        [SerializeField] private bool notifySensedObjExit = true;
        public bool GetNotifySensedObjExit => notifySensedObjExit;



        //When an object is detected, this event will invoked!
        public UnityEvent<Transform> onObjDetected;
        
        
        //When a detected object goes outside of the vision area, this event will invoked!
        public UnityEvent<Transform> onDetectedObjExit;
        
        
        //When an object is sensed, this event will invoked!
        public UnityEvent<Transform> onObjSensed;
        
        
        //When a sensed object goes outside of the sense field, this event will invoked!
        public UnityEvent<Transform> onSensedObjExit;
    }
}