using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Vision_Controller
{
    public class VisionController : MonoBehaviour
    {
        #region Variables


        [SerializeField] private VisionData data = new VisionData();
        public VisionData GetData => data;
        
        private Coroutine _coroutine;

        private WaitForSeconds _wait;

        private Vision _vision;
        
        
        
        
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
        
        private Vector3 _visionRelativePos;

        private float _projection;
        
        private float _senseProjection;
        
#endif

        #endregion

        

        
        
        #region Methods
        
        
        
        #region FunctionalityMethods

        
        private void OnEnable() => ConfigureVision();
        

        private void Start()
        {
            _wait = new WaitForSeconds(data.GetRecheckTime);
            _coroutine ??= StartCoroutine(CheckVision());
        }

        
        private void OnDisable() => ResetVisualizationColors();
        

        private void ConfigureVision() => _vision ??= data.GetVisionFactory.CreateVision(transform, data);

        
        
        private IEnumerator CheckVision()
        {
            while (enabled)
            {
                yield return _wait;

                _vision.ManageArea(data.GetCenter + transform.position, out bool isSeen, out bool isSensed);

                
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
            if (data.GetMaxHeight <= data.GetMinHeight + .1f) data.GetMinHeight += .1f;
            if (data.GetMaxRadius <= data.GetMinRadius + .1f) data.GetMinRadius += .1f;
        }

        private void OnDrawGizmos()
        {
            if(!visualize) return;
            
            ConfigureVision();
            CalculateVisionRelativePos();
            CalculateProjection();
            if(data.GetCalculateSense) CalculateSenseProjection();
            DrawVision();

            
            
            void CalculateVisionRelativePos() => _visionRelativePos = transform.position + data.GetCenter;
            void CalculateProjection() => _projection = Mathf.Cos((GetData.GetFov * .5f) * Mathf.Deg2Rad);
            void CalculateSenseProjection() => _senseProjection = Mathf.Cos((GetData.GetSenseField * .5f) * Mathf.Deg2Rad);
        }

        
        
        
        
        private void DrawVision()
        {
            if (GetData.GetCalculateSense)
            {
                ChangeColor(_senseVisualisationColor);
                _vision.DrawArea(_visionRelativePos, GetData.GetSenseField, _senseProjection);
            }
            
            ChangeColor(_visionVisualisationColor);
            _vision.DrawArea(_visionRelativePos, GetData.GetFov, _projection);
            
            
            
            
            void ChangeColor(Color color) => Gizmos.color = Handles.color = color;
        }
        


        private void ChangeVisionVisualizationColor(Color color) => _visionVisualisationColor = color;
        
        
        private void ChangeSenseVisualizationColor(Color color) => _senseVisualisationColor = color;
        
        
        
        public void ResetVisualizationColors()
        {
            ChangeVisionVisualizationColor(normalColor);
            ChangeSenseVisualizationColor(senseNormalColor);
        }

#endif

        #endregion
        

        #endregion
    }
}