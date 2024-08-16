#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace VisionController
{
    [CustomEditor(typeof(VisionController))]
    public class VisionControllerEditor : Editor
    {
        #region Variables

        
        private VisionController _visionController;

        private Texture2D _directionTexture;
        private Texture2D _icon;
        private VisionMode _mode;
        private int _direction;
        private float _recheckTime;

        private int _fov;
        private float _minRadius;
        private float _maxRadius;
        private float _height;

        private bool _drawGizmos;
        private Color _normalColor;
        private Color _detectedColor;

        private int _defaultGUISpace = 10;
        
        #endregion

        
        #region Methods

        
        
        private void OnEnable()
        {
            Init();
            LoadImages();
            SetIcon();
        }


        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(_visionController), typeof(VisionController), false);
            GUI.enabled = true;

            AddSpace(_defaultGUISpace / 2);
            AddTitle("Vision Setting", 13, Color.white);
            
            AddSpace(_defaultGUISpace);
            ShowCommonFields();

            AddSpace(_defaultGUISpace);
            ShowVisionSettingFields();

            AddSpace(_defaultGUISpace * 2);
            ShowEventFields();

            AddSpace(_defaultGUISpace * 3);
            AddTitle("Visualization Setting", 13, Color.white);
            
            AddSpace(_defaultGUISpace);
            ShowVisualizationFields();


            ApplyModifiedFields();
        }


        
        //Initialize default values from VisionController script
        private void Init()
        {
            _visionController = (VisionController) target;

            _mode = _visionController.GetMode;
            _direction = _visionController.GetDirection;
            _recheckTime = _visionController.GetRecheckTime;
            _fov = _visionController.GetFov;
            _minRadius = _visionController.GetMinRadius;
            _maxRadius = _visionController.GetMaxRadius;
            _height = _visionController.GetHeight;
            _drawGizmos = _visionController.GetDrawGizmos;
            _normalColor = _visionController.GetNormalColor;
            _detectedColor = _visionController.GetDetectedColor;
        }


        private void ShowCommonFields()
        {
            _mode = (VisionMode) EditorGUILayout.EnumPopup("Mode", _mode);
            AddTooltip("The vision modes determine how to calculate the vision!");
            
            AddSpace(_defaultGUISpace);
            
            ShowDirectionField();
        
            AddSpace(_defaultGUISpace * 2);
           
            _recheckTime = EditorGUILayout.FloatField("Recheck Time", _recheckTime);
            AddTooltip("This specifies that every few seconds it should check if any objects is in the vision!");
        }


        private void ShowDirectionField()
        {
            GUILayout.Label("Direction");
            
            EditorGUILayout.BeginHorizontal();

            if (_directionTexture != null)
            {
                EditorGUILayout.BeginVertical();
                Matrix4x4 originalMatrix = GUI.matrix;

                Vector2 scale = new Vector2(89, 89);
                Rect position = GUILayoutUtility.GetRect(scale.x, scale.y);

                GUIUtility.RotateAroundPivot(_direction, position.center);

                GUI.DrawTexture(position, _directionTexture);
                
                GUI.matrix = originalMatrix;

                GUILayout.BeginHorizontal();
                
                int space = 38;
                if (_direction > 10) space = 34;
                if (_direction > 100) space = 31;
                
                AddSpace(space);
                GUILayout.Label($"{_direction}°");
                GUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
            else
            {
                ShowErrorText("No direction image found.");
            }

            AddSpace(_defaultGUISpace * 3);

            GUILayout.BeginVertical();
            
            AddSpace(_defaultGUISpace * 4);
            _direction = EditorGUILayout.IntSlider("", _direction, 0, 360);
            
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        

        private void ShowVisionSettingFields()
        {
            switch (_mode)
            {
                case VisionMode.CylindricalVision:
                {
                    ShowCylindricalVisionFields();
                    break;
                }

                case VisionMode.SphericalVision:
                {
                    ShowSphericalVisionFields();
                    break;
                }

                case VisionMode.ConicalVision:
                {
                    ShowConicalVisionFields();
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void ShowCylindricalVisionFields()
        {
            _fov = EditorGUILayout.IntSlider("Fov", _fov, 0, 360);
            _height = EditorGUILayout.FloatField("Height", _height);
            
            _minRadius = EditorGUILayout.FloatField("Min Radius", _minRadius);
            AddTooltip("Any object closer than the min radius isn't detected!");
            
            
            _maxRadius = EditorGUILayout.FloatField("Max Radius", _maxRadius);
            AddTooltip("Any object further away than the max radius isn't detected!");
        }


        private void ShowSphericalVisionFields()
        {
            _minRadius = EditorGUILayout.FloatField("Min Radius", _minRadius);
            AddTooltip("Any object closer than the min radius isn't detected!");

            
            _maxRadius = EditorGUILayout.FloatField("Max Radius", _maxRadius);
            AddTooltip("Any object further away than the max radius isn't detected!");
        }


        private void ShowConicalVisionFields()
        {
            _fov = EditorGUILayout.IntSlider("Fov", _fov, 0, 360);
            _height = EditorGUILayout.FloatField("Height", _height);
            
            
            _minRadius = EditorGUILayout.FloatField("Min Radius", _minRadius);
            AddTooltip("Any object closer than the min radius isn't detected!");

            
            _maxRadius = EditorGUILayout.FloatField("Max Radius", _maxRadius);
            AddTooltip("Any object further away than the max radius isn't detected!");
        }


        private void ShowEventFields()
        {
            var onObjDetected = serializedObject.FindProperty("onObjDetected");
            EditorGUILayout.PropertyField(onObjDetected, true);
            AddTooltip("When an object is detected, this event will invoked!");

            var onObjExit = serializedObject.FindProperty("onObjExit");
            EditorGUILayout.PropertyField(onObjExit, true);
            AddTooltip("When a detected object goes outside of the vision area, this event will invoked!");
        }


        private void ShowVisualizationFields()
        {
            _drawGizmos = EditorGUILayout.Toggle("Draw Gizmos", _drawGizmos);

            if (!_drawGizmos) return;

            AddSpace(_defaultGUISpace);

            _normalColor = EditorGUILayout.ColorField("Normal Color", _normalColor);
            AddTooltip("The normal color of the visualization");
            
            
            _detectedColor = EditorGUILayout.ColorField("Detected Color", _detectedColor);
            AddTooltip("The color of the visualization when something detected");
        }
        

        private void ShowErrorText(string message)
        {
            GUIStyle style = new GUIStyle()
                {fontStyle = FontStyle.Bold, fontSize = 13, normal = new GUIStyleState() {textColor = Color.red}};
                
            AddSpace(_defaultGUISpace * 4);
                
            GUILayout.Label(message, style);
                
            AddSpace(_defaultGUISpace * 4);
        }


        private void AddTitle(string text, int fontSize, Color color)
        {
            GUIStyle style = new GUIStyle()
                {fontStyle = FontStyle.Bold, fontSize = fontSize, normal = new GUIStyleState() {textColor = color}};
            
            EditorGUILayout.LabelField(text, style);
        }
        
        
        private void AddSpace(int amount) => GUILayout.Space(amount);
        
        
        private void AddTooltip(string text)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.Label(rect, new GUIContent("", text));
        }


        private void LoadImages()
        {
            string scriptPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(_visionController)));
            string directory = Directory.GetParent(Directory.GetParent(scriptPath).FullName).FullName;
            string relativeParentDirectory = Path.GetRelativePath(Application.dataPath, directory);
            string imagePath = Path.Combine("Assets", relativeParentDirectory, "Images", "Direction View.png");
            _directionTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
            
            scriptPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(_visionController)));
            directory = Directory.GetParent(Directory.GetParent(scriptPath).FullName).FullName;
            relativeParentDirectory = Path.GetRelativePath(Application.dataPath, directory);
            imagePath = Path.Combine("Assets", relativeParentDirectory, "Images", "Vision Controller Icon.png");
            _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
        }
        
        
        private void SetIcon() => EditorGUIUtility.SetIconForObject(target, _icon);

        
        
        private void ApplyModifiedFields()
        {
            _visionController.ApplyModifiedFields(_mode, _direction, _recheckTime, _fov, _minRadius, _maxRadius, _height,
                _drawGizmos, _normalColor, _detectedColor);
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}


#endif