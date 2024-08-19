using System;
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

        
        
        //This specifies that every few seconds it should check if any objects is in the vision!
        [SerializeField]  private float recheckTime = .03f;

        
        [Range(0, 360)]
        [SerializeField] private int fov = 60;
        
        
        //Any object closer than the min radius isn't detected!
        [SerializeField] private float minRadius = .5f;
        
        
        //Any object further away than the max radius isn't detected!
        [SerializeField] private float maxRadius = 2f;


        
        [SerializeField] private float minHeight = 0;

        
        
        [SerializeField] private float maxHeight = 3;
        
        
        
        //When an object is detected, this event will invoked!
        public UnityEvent<Transform> onObjDetected;
        
        
        //When a detected object goes outside of the vision area, this event will invoked!
        public UnityEvent<Transform> onObjExit;


        
        
        
        
        private float _projectile;
        

        
        
        
#if UNITY_EDITOR

        [SerializeField] private bool drawGizmos = true;
        public bool GetDrawGizmos => drawGizmos;

        
        //The normal color of the visualization
        [SerializeField] private Color normalColor = Color.white;

        
        //The color of the visualization when something detected
        [SerializeField] private Color detectedColor = Color.red;
        
#endif

        #endregion

        

        #region Methods

        
        // ReSharper disable once ParameterHidesMember
        public void ApplyModifiedFields(VisionMode mode, int direction, float recheckTime, int fov, float minRadius,
            float maxRadius, float minHeight, float maxHeight, bool drawGizmos, Color normalColor, Color detectedColor)
        {
            this.mode = mode;
            this.direction = direction;
            this.recheckTime = recheckTime;
            this.fov = fov;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;

            this.drawGizmos = drawGizmos;
            this.normalColor = normalColor;
            this.detectedColor = detectedColor;
        }


        private void OnDrawGizmos()
        {
            if(!drawGizmos) return;

            _projectile = Mathf.Cos((fov * .5f) * Mathf.Deg2Rad);
            
            ConfigureMatrices(transform, Vector3.up, direction);
            DrawVision();
        }

        
        private static void ConfigureMatrices(Transform baseTransform, Vector3 axis, float degree)
        {
            Gizmos.matrix = Handles.matrix = baseTransform.localToWorldMatrix;
            Gizmos.matrix = Handles.matrix = MathHelper.RotationMatrix(default, axis, degree);
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
            float x = MathHelper.Pythagoras_UnknownSide(1, _projectile);
            Vector3 rightVec = new Vector3(x, 0, _projectile) * maxRadius;
            Vector3 leftVec = new Vector3(-x, 0, _projectile) * maxRadius;
            
            Gizmos.DrawLine(default, rightVec * maxRadius);
            Gizmos.DrawLine(default, leftVec * maxRadius);
        }
        
        private void DrawSphericalVision()
        {
            
        }
        
        private void DrawConicalVision()
        {
            
        }
        
        #endregion
    }
}