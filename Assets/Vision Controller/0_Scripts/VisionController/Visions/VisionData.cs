using UnityEngine;
using UnityEngine.Events;

namespace Vision_Controller
{
    [System.Serializable]
    public class VisionData
    {
        //The vision modes determine how to calculate the vision
        [SerializeField] private VisionFactory visionFactory;
        public VisionMode GetMode => visionFactory ? visionFactory.Mode() : VisionMode.CylindricalVision;
        public VisionFactory GetVisionFactory => visionFactory;

        
        [SerializeField] private LayerMask targetLayer;
        public LayerMask GetTargetLayer => targetLayer;
        
        
        
        [SerializeField] private LayerMask obstaclesLayer;
        public LayerMask GetObstaclesLayer => obstaclesLayer;

        
        [Range(0, 360)]
        [SerializeField] private int direction = 90;
        public int GetDirection => direction;
        
        
        
        [SerializeField] private Vector3 center = Vector3.zero;
        public Vector3 GetCenter => center;

        
        
        
        //This specifies that every few seconds it should check if any objects is in the vision!
        [SerializeField] private float recheckTime = .03f;
        public float GetRecheckTime => recheckTime;



        [Range(0, 360)]
        [SerializeField] private int fov = 60;
        public int GetFov => fov;
        
        
        [Range(0, 360)]
        [SerializeField] private int senseField = 200;
        public int GetSenseField => senseField;

        
        
        //Any object closer than the min radius isn't detected!
        [SerializeField] private float minRadius = .7f;
        public float GetMinRadius { get => minRadius; set => minRadius = value; }


        //Any object further away than the max radius isn't detected!
        [SerializeField] private float maxRadius = 5f;
        public float GetMaxRadius => maxRadius;

        

        //Maximum objects that can be detected!
        [SerializeField] private int maxObjDetection = 10;
        public int GetMaxObjDetection => maxObjDetection;



        [SerializeField] private float minHeight = -.7f;
        public float GetMinHeight { get => minHeight; set => minHeight = value; }

        

        [SerializeField] private float maxHeight = 1.3f;
        public float GetMaxHeight => maxHeight;
        
        
        
        [SerializeField] private bool notifyObjExit = true;
        public bool GetNotifyObjExit => notifyObjExit;
        
        
        
        [SerializeField] private bool blockCheck = true;
        public bool GetBlockCheck => blockCheck;
        
        
        [SerializeField] private bool calculateSense = true;
        public bool GetCalculateSense => calculateSense;
        
        
        [SerializeField] private bool notifySensedObjExit = true;
        public bool GetNotifySensedObjExit => notifySensedObjExit;



        //When an object is detected, this event will invoked!
        public UnityEvent<Transform> onObjDetected;
        
        
        //When a detected object goes outside of the vision area, this event will invoked!
        public UnityEvent<Transform> onObjExit;
        
        
        //When an object is sensed, this event will invoked!
        public UnityEvent<Transform> onObjSensed;
        
        
        //When a sensed object goes outside of the sense field, this event will invoked!
        public UnityEvent<Transform> onSensedObjExit;
    }
}