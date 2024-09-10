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
        public LayerMask GetObstaclesLayer => obstaclesLayer;

        
        [Range(0, 360)]
        [SerializeField] private int direction = 90;
        public int GetDirection => direction;
        
        
        
        [SerializeField] private Vector3 center = Vector3.zero;
        public Vector3 GetCenter => center;

        
        
        
        //This specifies that every few seconds it should check if any objects is in the vision!
        [SerializeField] private float recheckTime = .03f;
        
        
        
        [Range(0, 360)]
        [SerializeField] private int fov = 60;
        public int GetFov => fov;
        
        
        [Range(0, 360)]
        [SerializeField] private int senseField = 200;
        public int GetSenseField => senseField;

        
        
        //Any object closer than the min radius isn't detected!
        [SerializeField] private float minRadius = .7f;
        public float GetMinRadius => minRadius;



        //Any object further away than the max radius isn't detected!
        [SerializeField] private float maxRadius = 5f;
        public float GetMaxRadius => maxRadius;

        

        //Maximum objects that can be detected!
        [SerializeField] private int maxObjDetection = 10;
        public int GetMaxObjDetection => maxObjDetection;



        [SerializeField] private float minHeight = -.7f;
        public float GetMinHeight => minHeight;



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
        
        
#if UNITY_EDITOR

        [SerializeField] private bool visualize = true;
        public bool GetVisualize => visualize;

        
        //The normal color of the visualisation
        [SerializeField] private Color normalColor = Color.white;

        
        //The color of the visualisation when something detected
        [SerializeField] private Color detectedColor = Color.red;
        
        //The normal color of the sense field visualisation
        [SerializeField] private Color senseNormalColor = Color.grey;

        
        //The color of the sense field visualisation when something detected
        [SerializeField] private Color sensedColor = Color.yellow;


        
        private static Color _visionVisualisationColor = Color.white;
        
        private static Color _senseVisualisationColor = Color.grey;
        
        private float _projection;
        
        private float _senseProjection;
        
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
                    _vision = new SphericalVision(this);
                    break;
                }
                case VisionMode.ConicalVision:
                {
                    _vision = new ConicalVision(this);
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

        
        private void OnDisable() => ResetVisualizationColors();


        private IEnumerator CheckVision()
        {
            while (enabled)
            {
                yield return _wait;

                _vision.CheckVisionArea(center + transform.position, out bool isSeen, out bool isSensed);

                
#if UNITY_EDITOR
                ChangeVisionVisualizationColor(isSeen ? detectedColor : normalColor);
                ChangeSenseVisualizationColor(isSensed ? sensedColor : senseNormalColor);
#endif
            }
            
            StopCoroutine(_coroutine);
        }
        

        #endregion
        
        
        
        
        
        #region visualisation Methods

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
            if(calculateSense) CalculateSenseProjection();
            
            DrawVision();
        }

        
        
        private void CalculateProjection() => _projection = Mathf.Cos((fov * .5f) * Mathf.Deg2Rad);
        private void CalculateSenseProjection() => _senseProjection = Mathf.Cos((senseField * .5f) * Mathf.Deg2Rad);


        

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
            
            
            if (calculateSense)
            {
                ChangeColor(_senseVisualisationColor);

                pos.y = minHeight + _visionRelativePos.y;

                ConfigureMatrices(pos, Vector3.up, direction, rotation);
                DrawArea(senseField ,_senseProjection);
            
                pos.y = maxHeight + _visionRelativePos.y;

                ConfigureMatrices(pos, Vector3.up, direction, rotation);
                DrawArea(senseField, _senseProjection,true);
            }

            ChangeColor(_visionVisualisationColor);

            pos.y = minHeight + _visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, direction, rotation);
            DrawArea(fov, _projection);
            
            pos.y = maxHeight + _visionRelativePos.y;

            ConfigureMatrices(pos, Vector3.up, direction, rotation);
            DrawArea(fov, _projection,true);
        }
        
        private void DrawSphericalVision()
        {
            Gizmos.DrawWireSphere(_visionRelativePos, minRadius);
            Gizmos.DrawWireSphere(_visionRelativePos, maxRadius);
        }
        
        private void DrawConicalVision()
        {
            Quaternion rotation = transform.rotation;

            if (calculateSense)
            {
                ChangeColor(_senseVisualisationColor);

                ConfigureMatrices(_visionRelativePos, Vector3.up, direction, rotation);
                DrawArea(senseField, _senseProjection);

                rotation *= Quaternion.AngleAxis((-direction + 90), Vector3.up);
            
                Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(_visionRelativePos, Vector3.forward, 90, rotation);
                DrawArea(senseField, _senseProjection, false, true);
            }
            
            
            ChangeColor(_visionVisualisationColor);

            ConfigureMatrices(_visionRelativePos, Vector3.up, direction, rotation);
            DrawArea(fov, _projection);

            rotation *= Quaternion.AngleAxis((-direction + 90), Vector3.up);
            
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(_visionRelativePos, Vector3.forward, 90, rotation);
            DrawArea(fov, _projection, false, true);
        }


        private void DrawArea(int area, float projection, bool connectVertices = false, bool drawDisk = false)
        {
            if(area == senseField && senseField <= fov) return;
            
            if(area is 0) return;
            
            float x = MathHelper.Pythagoras_UnknownSide(1, projection);
            
            Vector3 minRight = new Vector3(x, 0, projection) * minRadius;
            Vector3 minLeft = new Vector3(-x, 0, projection) * minRadius;
            Vector3 maxRight = new Vector3(x, 0, projection) * maxRadius;
            Vector3 maxLeft = new Vector3(-x, 0, projection) * maxRadius;

            Handles.DrawWireArc(default, Vector3.up, minLeft, area, minRadius);
            Handles.DrawWireArc(default, Vector3.up, maxLeft, area, maxRadius);
            
            if(area is 360) return;

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
                if(area is 0 or 360) return;

                Vector3 diskCenter = new Vector3(0, 0, projection);
            
                Handles.DrawWireDisc(diskCenter * minRadius, Vector3.forward, x * minRadius);
                Handles.DrawWireDisc(diskCenter * maxRadius, Vector3.forward, x * maxRadius);
            }
        }


        
        private void ChangeVisionVisualizationColor(Color color) => _visionVisualisationColor = color;
        private void ChangeSenseVisualizationColor(Color color) => _senseVisualisationColor = color;
        
        public void ResetVisualizationColors()
        {
            ChangeVisionVisualizationColor(normalColor);
            ChangeSenseVisualizationColor(senseNormalColor);
        }
        
        void ChangeColor(Color color)
        {
            Gizmos.color = Handles.color = color;
        }
        
#endif

        #endregion
        

        #endregion
    }
}