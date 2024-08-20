using System;
using System.Collections.Generic;
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

        
        [Range(0, 360)]
        [SerializeField] private int direction = 0;
        public int GetDirection => direction;
        
        
        
        [SerializeField] private Vector3 center = Vector3.zero;
        
        
        
        //This specifies that every few seconds it should check if any objects is in the vision!
        [SerializeField]  private float recheckTime = .03f;

        
        [Range(0, 360)]
        [SerializeField] private int fov = 60;
        
        
        //Any object closer than the min radius isn't detected!
        [SerializeField] private float minRadius = .5f;
        
        
        //Any object further away than the max radius isn't detected!
        [SerializeField] private float maxRadius = 2f;


        
        [SerializeField] private float minHeight = 0;

        
        
        [SerializeField] private float maxHeight = 1.3f;
        
        
        
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
        
#endif

        
        
        
        private Vector3 _matrixReletivePos;
        
        private float _projectile;
        
        
        #endregion

        

        #region Methods

        private void OnEnable()
        {
            CalculateProjectile();
        }

        private void CalculateProjectile() => _projectile = Mathf.Cos((fov * .5f) * Mathf.Deg2Rad);

        
        
        public void ValidateValues()
        {
            if (maxHeight <= minHeight + .1f) maxHeight = minHeight + .1f;
            if (maxRadius <= minRadius + .1f) maxRadius = minRadius + .1f;
        }
        
        
        #region Visialization Methods

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(!visualize) return;
         
            CalculateProjectile();
            ChangeVisualizationColor(normalColor);
            ConfigureMatrices(transform, Vector3.up, direction);
            DrawVision();
        }


        private void ConfigureMatrices(Transform baseTransform, Vector3 axis, float degree)
        {
            _matrixReletivePos = baseTransform.TransformPoint(center);
            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(_matrixReletivePos, axis, degree, baseTransform.rotation);
        }
        
        
        
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

        private void DrawCylindricalVision()
        {
            Transform tran = transform;
            Quaternion rotation = tran.rotation;
            Vector3 pos = _matrixReletivePos;
            pos.y = minHeight + center.y;

            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(pos, Vector3.up, direction, rotation);
            DrawVisionArea();
            
            pos.y = maxHeight + center.y;

            Gizmos.matrix = Handles.matrix = MathHelper.ChangeMatrix(pos, Vector3.up, direction, rotation);
            DrawVisionArea(true);
        }
        
        private void DrawSphericalVision()
        {
            
        }
        
        private void DrawConicalVision()
        {
            
        }


        private void DrawVisionArea(bool connectVertices = false)
        {
            if(fov is 0) return;
            
            float x = MathHelper.Pythagoras_UnknownSide(1, _projectile);
            
            Vector3 minRight = new Vector3(x, 0, _projectile) * minRadius;
            Vector3 minLeft = new Vector3(-x, 0, _projectile) * minRadius;
            Vector3 maxRight = new Vector3(x, 0, _projectile) * maxRadius;
            Vector3 maxLeft = new Vector3(-x, 0, _projectile) * maxRadius;

            Handles.DrawWireArc(default, Vector3.up, minLeft, fov, minRadius);
            Handles.DrawWireArc(default, Vector3.up, maxLeft, fov, maxRadius);
            
            if(fov is 360) return;

            Gizmos.DrawLine(minRight, maxRight);
            Gizmos.DrawLine(minLeft, maxLeft);
            
            if(!connectVertices) return;

            float height = -(maxHeight - minHeight);
            Gizmos.DrawLine(minRight, new Vector3(minRight.x, height, minRight.z));
            Gizmos.DrawLine(minLeft, new Vector3(minLeft.x, height, minLeft.z));
            Gizmos.DrawLine(maxRight, new Vector3(maxRight.x, height, maxRight.z));
            Gizmos.DrawLine(maxLeft, new Vector3(maxLeft.x, height, maxLeft.z));
        }


        private void ChangeVisualizationColor(Color color) => Gizmos.color = Handles.color = color;

#endif

        #endregion


        #endregion
    }
}