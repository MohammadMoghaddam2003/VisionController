using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Vision_Controller
{
    public class VisionController : MonoBehaviour
    {
        #region Variables

        //The vision modes determine how to calculate the vision
        [SerializeField] private VisionMode mode = VisionMode.CylindricalVision;
        public VisionMode GetMode => mode;

        
        [SerializeField] private LayerMask targetLayer;
        public LayerMask GetTargetLayer => targetLayer;
        
        
        
        [SerializeField] private LayerMask obstaclesLayer;
        public LayerMask GetObstaclesLayer=> obstaclesLayer;

        
        [Range(0, 360)]
        [SerializeField] private int direction = 90;
        public int GetDirection => direction;
        
        
        
        [SerializeField] private Vector3 center = Vector3.zero;
        public Vector3 GetCenter => center;

        
        
        
        //This specifies that every few seconds it should check if any objects is in the vision!
        [SerializeField] private float recheckTime = .03f;
        
        
        
        [Range(0, 360)]
        [SerializeField] private int fov = 60;
        public int GetFov=> fov;

        
        
        //Any object closer than the min radius isn't detected!
        [SerializeField] private float minRadius = .7f;
        public float GetMinRadius => minRadius;



        //Any object further away than the max radius isn't detected!
        [SerializeField] private float maxRadius = 5f;
        public float GetMaxRadius => maxRadius;

        

        //Maximum objects that can be detected!
        [SerializeField] private int maxObjDetection = 5;
        public int GetMaxObjDetection => maxObjDetection;



        [SerializeField] private float minHeight = -.7f;
        public float GetMinHeight => minHeight;



        [SerializeField] private float maxHeight = 1.3f;
        public float GetMaxHeight => maxHeight;
        
        
        
        [SerializeField] private bool notifyObjExit = true;
        public bool GetNotifyObjExit => notifyObjExit;
        
        
        
        [SerializeField] private bool blockCheck = true;
        public bool GetBlockCheck=> blockCheck;



        //When an object is detected, this event will invoked!
        public UnityEvent<Transform> onObjDetected;
        
        
        //When a detected object goes outside of the vision area, this event will invoked!
        public UnityEvent<Transform> onObjExit;
        
        
#if UNITY_EDITOR

        [SerializeField] private bool visualize = true;
        public bool GetVisualize => visualize;

        
        //The normal color of the visualization
        [SerializeField] private Color normalColor = Color.white;

        
        //The color of the visualization when something detected
        [SerializeField] private Color detectedColor = Color.red;


        
        private static Color _visualizationColor = Color.white;
        
        private float _projection;
        
        private Vector3 _visionRelativePos;

#endif



        
        private Coroutine _coroutine;

        private WaitForSeconds _wait;

        private AbstractVision _vision;
        
        #endregion

        

        #region Methods
        
        
        
        #region FunctionalityMethods

        
        private void OnEnable() => ConfigureVision();
        
        

        private void ConfigureVision()
        {
            switch (mode)
            {
                case VisionMode.CylindricalVision:
                {
                    _vision = new CylindricalVision(this);
                    break;
                }
                case VisionMode.SphericalVision:
                {
                    break;
                }
                case VisionMode.ConicalVision:
                {
                    break;
                }
                
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        

        private void Start()
        {
            _wait = new WaitForSeconds(recheckTime);
            _coroutine ??= StartCoroutine(CheckVision());
        }

        

        private IEnumerator CheckVision()
        {
            while (enabled)
            {
                yield return _wait;
                
                if(_vision.CheckVisionArea(center + transform.position))
                {
                    ChangeVisualizationColor();    
                }
                else
                {
                    ResetVisualizationColor();
                }
            }
            
            StopCoroutine(_coroutine);
        }


        private void OnDisable() => ResetVisualizationColor();

        #endregion
        
        
        
        
        
        #region Visialization Methods

#if UNITY_EDITOR
        
        public void ValidateValues()
        {
            if (maxHeight <= minHeight + .1f) maxHeight = minHeight + .1f;
            if (maxRadius <= minRadius + .1f) maxRadius = minRadius + .1f;
        }

        private void OnDrawGizmos()
        {
            if(!visualize) return;
            
            _visionRelativePos = transform.position + center;

            CalculateProjection();
            SetVisualizationColor();
            DrawVision();
        }

        
        
        private void CalculateProjection() => _projection = Mathf.Cos((fov * .5f) * Mathf.Deg2Rad);


        

        private void ConfigureMatrices(Vector3 pos, Vector3 axis, float degree, Quaternion rotation) => 
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(pos, axis, (-degree + 90), rotation);
        
        
        
        
        private void DrawVision()
        {
            switch (mode)
            {
                case VisionMode.CylindricalVision:
                {
                    DrawCylindricalVision();
                    break;
                }
                case VisionMode.SphericalVision:
                {
                    DrawSphericalVision();
                    break;
                }
                case VisionMode.ConicalVision:
                {
                    DrawConicalVision();
                    break;
                }
                
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawCylindricalVision()
        {
            Transform tran = transform;
            Quaternion rotation = tran.rotation;
            Vector3 pos = _visionRelativePos;
            pos.y = minHeight + _visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, direction, rotation);
            DrawVisionArea();
            
            pos.y = maxHeight + _visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, direction, rotation);
            DrawVisionArea(true);
        }
        
        private void DrawSphericalVision()
        {
            Gizmos.DrawWireSphere(default, minRadius);
            Gizmos.DrawWireSphere(default, maxRadius);
        }
        
        private void DrawConicalVision()
        {
            Transform tran = transform;
            Quaternion rotation = tran.rotation;

            ConfigureMatrices(_visionRelativePos, Vector3.up, direction, rotation);
            DrawVisionArea();

            rotation *= Quaternion.AngleAxis(direction, Vector3.up);
            
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(_visionRelativePos, Vector3.forward, 90, rotation);
            DrawVisionArea(false, true);
        }


        private void DrawVisionArea(bool connectVertices = false, bool drawDisk = false)
        {
            if(fov is 0) return;
            
            float x = MathHelper.Pythagoras_UnknownSide(1, _projection);
            
            Vector3 minRight = new Vector3(x, 0, _projection) * minRadius;
            Vector3 minLeft = new Vector3(-x, 0, _projection) * minRadius;
            Vector3 maxRight = new Vector3(x, 0, _projection) * maxRadius;
            Vector3 maxLeft = new Vector3(-x, 0, _projection) * maxRadius;

            Handles.DrawWireArc(default, Vector3.up, minLeft, fov, minRadius);
            Handles.DrawWireArc(default, Vector3.up, maxLeft, fov, maxRadius);
            
            if(fov is 360) return;

            Gizmos.DrawLine(minRight, maxRight);
            Gizmos.DrawLine(minLeft, maxLeft);

            if (!connectVertices)
            {
                if(!drawDisk) return;
                else DrawDisk();
                    
                return;
            }

            float height = -(maxHeight - minHeight);
            Gizmos.DrawLine(minRight, new Vector3(minRight.x, height, minRight.z));
            Gizmos.DrawLine(minLeft, new Vector3(minLeft.x, height, minLeft.z));
            Gizmos.DrawLine(maxRight, new Vector3(maxRight.x, height, maxRight.z));
            Gizmos.DrawLine(maxLeft, new Vector3(maxLeft.x, height, maxLeft.z));

            void DrawDisk()
            {
                if(fov is 0 or 360) return;

                Vector3 diskCenter = new Vector3(0, 0, _projection);
            
                Handles.DrawWireDisc(diskCenter * minRadius, Vector3.forward, x * minRadius);
                Handles.DrawWireDisc(diskCenter * maxRadius, Vector3.forward, x * maxRadius);
            }
        }


        private void SetVisualizationColor() => Gizmos.color = Handles.color = _visualizationColor;
        
        private void ResetVisualizationColor() => _visualizationColor = normalColor;
        
        private void ChangeVisualizationColor() => _visualizationColor = detectedColor;
        
#endif

        #endregion
        

        #endregion
    }
}