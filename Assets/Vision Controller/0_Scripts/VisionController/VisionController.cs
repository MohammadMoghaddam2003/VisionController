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

        private float _visionProjection;
        
        private float _senseProjection;
        
#endif

        #endregion

        
        
        
        #region Methods
        
        
        
        #region FunctionalityMethods

        
        
        private void OnEnable() => InitializeVision();
        

        private void Start()
        {
            _wait = new WaitForSeconds(data.GetRecheckTime);
            _coroutine ??= StartCoroutine(CheckVision());
        }

        
        private void OnDisable() => ResetVisualizationColors();
        

        private void InitializeVision()
        {
            if((!Application.isPlaying && visualize) || _vision is null)
                _vision = data.GetVisionFactory.CreateVision(transform, data);
        }


        private IEnumerator CheckVision()
        {
            bool isSeen;
            bool isSensed;
            
            
            while (enabled)
            {
                yield return _wait;

                _vision.ManageArea(data.GetCenter + transform.position, out isSeen, out isSensed);

                
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
        
        /// <summary>
        /// Checks the max height and max radius have not been less than their min values!
        /// </summary>
        public void ValidateValues()
        {
            if (data.GetMaxHeight <= data.GetMinHeight + .1f) data.GetMaxHeight = data.GetMinHeight + .1f;
            if (data.GetMaxRadius <= data.GetMinRadius + .1f) data.GetMaxRadius = data.GetMinRadius + .1f;
        }

        
        private void OnDrawGizmos()
        {
            if(!visualize) return;
            
            InitializeVision();
            CalculateVisionRelativePos();
            CalculateVisionProjection();
            CalculateSenseProjection();
            DrawVision();

            
            
            
            // Calculates the position of the vision based on the Center's amount  in the Inspector and the object position
            void CalculateVisionRelativePos() => _visionRelativePos = transform.position + data.GetCenter;
            
            
            // Calculates the projection of the field of view degree
            void CalculateVisionProjection() =>
                _visionProjection = MathHelper.CalculateProjection(GetData.GetFov * .5f); 
            

            // Calculates the projection of the field of sense degree
            void CalculateSenseProjection()
            {
                if(!data.GetCalculateSense) return;
                _senseProjection = MathHelper.CalculateProjection(GetData.GetFos * .5f);
            }
        }
        
        
        private void DrawVision()
        {
            if (GetData.GetCalculateSense)
            {
                ChangeColor(_senseVisualisationColor);
                _vision.DrawArea(_visionRelativePos, GetData.GetFos, _senseProjection);
            }
            
            ChangeColor(_visionVisualisationColor);
            _vision.DrawArea(_visionRelativePos, GetData.GetFov, _visionProjection);
            
            
            
            
            void ChangeColor(Color color) => Gizmos.color = Handles.color = color;
        }
        

        private void ChangeVisionVisualizationColor(Color color) => _visionVisualisationColor = color;
        
        
        private void ChangeSenseVisualizationColor(Color color) => _senseVisualisationColor = color;
        
        
        /// <summary>
        /// Resets the visualization colors to their normal colors
        /// </summary>
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