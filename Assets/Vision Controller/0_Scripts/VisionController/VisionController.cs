using UnityEngine;
using UnityEngine.Events;

namespace VisionController
{
    public class VisionController : MonoBehaviour
    {
        #region Variables

        //The vision modes determine how to calculate the vision
        private VisionMode _mode = VisionMode.CylindricalVision;
        public VisionMode GetMode
        {
            get => _mode ; 
            private set => _mode = value;
        }

        
        
        private int _direction;
        public int GetDirection
        {
            get => _direction; 
            private set => _direction = value;
        }

        
        
        //This specifies that every few seconds it should check if any objects is in the vision!
        private float _recheckTime = .3f;
        public float GetRecheckTime
        {
            get => _recheckTime ; 
            private set => _recheckTime = value;
        }
        
        
        
        private int _fov = 60;
        public int GetFov
        {
            get => _fov ; 
            private set => _fov = value;
        }
        
        

        //Any object closer than the min radius isn't detected!
        private float _minRadius = .5f;
        public float GetMinRadius
        {
            get => _minRadius ; 
            private set => _minRadius = value;
        }
        

        
        //Any object further away than the max radius isn't detected!
        private float _maxRadius = 3f;
        public float GetMaxRadius
        {
            get => _maxRadius ; 
            private set => _maxRadius = value;
        }

        
        
        private float _height = 2f;
        public float GetHeight
        {
            get => _height ; 
            private set => _height = value;
        }
        
        

        //When an object is detected, this event will invoked!
        public UnityEvent<Transform> onObjDetected;


        //When a detected object goes outside of the vision area, this event will invoked!
        public UnityEvent<Transform> onObjExit;

        
        
        

#if UNITY_EDITOR
        
        private bool _drawGizmos = true;
        public bool GetDrawGizmos
        {
            get => _drawGizmos ; 
            private set => _drawGizmos = value;
        }

        
        //The normal color of the visualization
        private Color _normalColor = Color.white;
        public Color GetNormalColor
        {
            get => _normalColor ; 
            private set => _normalColor = value;
        }

        
        
        //The color of the visualization when something detected
        private Color _detectedColor = Color.red;
        public Color GetDetectedColor
        {
            get => _detectedColor ; 
            private set => _detectedColor = value;
        }
        
#endif

        #endregion

        

        #region Methods

        
        public void ApplyModifiedFields(VisionMode mode, int direction, float recheckTime, int fov, float minRadius,
            float maxRadius, float height, bool drawGizmos, Color normalColor, Color detectedColor)
        {
            _mode = mode;
            _direction = direction;
            _recheckTime = recheckTime;
            _fov = fov;
            _minRadius = minRadius;
            _maxRadius = maxRadius;
            _height = height;

            _drawGizmos = drawGizmos;
            _normalColor = normalColor;
            _detectedColor = detectedColor;
        }
        
        

        #endregion
    }
}