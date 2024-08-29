#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Vision_Controller
{
    [CustomEditor(typeof(VisionController))]
    public class VisionControllerEditor : Editor
    {
        #region Variables

        
        private VisionController _visionController;

        private Texture2D _directionTexture;
        private Texture2D _icon;
        
        private SerializedProperty _mode;
        private SerializedProperty _direction;
        private SerializedProperty _center;
        private SerializedProperty _recheckTime;
        private SerializedProperty _targetLayer;
        private SerializedProperty _obstaclesLayer;
        private SerializedProperty _fov;
        private SerializedProperty _minRadius;
        private SerializedProperty _maxRadius;
        private SerializedProperty _maxObjDetection;
        private SerializedProperty _minHeight;
        private SerializedProperty _maxHeight;
        private SerializedProperty _notifyObjExit;
        private SerializedProperty _blockCheck;
        private SerializedProperty _onObjDetected;
        private SerializedProperty _onObjExit;
        private SerializedProperty _visualize;
        private SerializedProperty _normalColor;
        private SerializedProperty _detectedColor;

        
        private int _defaultGUISpace = 10;
        
        #endregion

        
        #region Methods

        
        
        private void OnEnable()
        {
            Init();
            LoadImages();
            SetIcon();
        }

        private void Init()
        {
            _visionController ??= (VisionController) target;
            
            _mode = serializedObject.FindProperty("mode");
            _direction = serializedObject.FindProperty("direction");
            _center = serializedObject.FindProperty("center");
            _recheckTime = serializedObject.FindProperty("recheckTime");
            _maxObjDetection = serializedObject.FindProperty("maxObjDetection");
            _targetLayer = serializedObject.FindProperty("targetLayer");
            _obstaclesLayer = serializedObject.FindProperty("obstaclesLayer");
            _fov = serializedObject.FindProperty("fov");
            _minRadius = serializedObject.FindProperty("minRadius");
            _maxRadius = serializedObject.FindProperty("maxRadius");
            _minHeight = serializedObject.FindProperty("minHeight");
            _maxHeight = serializedObject.FindProperty("maxHeight");
            _notifyObjExit = serializedObject.FindProperty("notifyObjExit");
            _blockCheck = serializedObject.FindProperty("blockCheck");
            _onObjDetected = serializedObject.FindProperty("onObjDetected");
            _onObjExit = serializedObject.FindProperty("onObjExit");
            _visualize = serializedObject.FindProperty("visualize");
            _normalColor = serializedObject.FindProperty("normalColor");
            _detectedColor = serializedObject.FindProperty("detectedColor");
        }


        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(_visionController),
                typeof(VisionController), false);
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
        


        private void ShowCommonFields()
        {
            EditorGUILayout.PropertyField(_mode, true);
            AddTooltip("The vision modes determine how to calculate the vision!");
            
            EditorGUILayout.PropertyField(_targetLayer, true);
            
            EditorGUILayout.PropertyField(_obstaclesLayer, true);
            
            EditorGUILayout.PropertyField(_recheckTime, true);
            AddTooltip("This specifies that every few seconds it should check if any objects is in the vision!");
            
            EditorGUILayout.PropertyField(_maxObjDetection, true);
            AddTooltip("Maximum objects that can be detected!");

            AddSpace(_defaultGUISpace / 3);

            EditorGUILayout.PropertyField(_center, true);
        }


        private void ShowDirectionField()
        {
            GUILayout.Label("Direction");
            
            EditorGUILayout.BeginHorizontal();

            if (_directionTexture != null)
            {
                EditorGUILayout.BeginVertical();
                Matrix4x4 originalMatrix = GUI.matrix;

                float padding = 20f;
                float imageWidth = 89f;
                float imageHeight = 89f;

                Rect position = GUILayoutUtility.GetRect(imageWidth, imageHeight, GUILayout.Width(imageWidth));

                position.x = padding;
                position.width = imageWidth;
                position.height = imageHeight;

                int dir = -_visionController.GetDirection + 90;
                GUIUtility.RotateAroundPivot(dir, position.center);

                GUI.DrawTexture(position, _directionTexture, ScaleMode.ScaleToFit);
                
                GUI.matrix = originalMatrix;

                GUILayout.BeginHorizontal();
                
                int space = 40;
                if (dir > 10) space = 36;
                if (dir > 100) space = 33;
                
                AddSpace(space);
                GUILayout.Label($"{_visionController.GetDirection}°");
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
            EditorGUILayout.PropertyField(_direction, new GUIContent(""));
            
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        

        private void ShowVisionSettingFields()
        {
            switch (_visionController.GetMode)
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
            
            AddSpace(_defaultGUISpace);
            EditorGUILayout.PropertyField(_notifyObjExit, true);
            EditorGUILayout.PropertyField(_blockCheck, true);
        }


        private void ShowCylindricalVisionFields()
        {
            EditorGUILayout.PropertyField(_fov, new GUIContent("Fov"));
            EditorGUILayout.PropertyField(_minHeight, new GUIContent("Min Height"));
            EditorGUILayout.PropertyField(_maxHeight, new GUIContent("Max Height"));
            
            EditorGUILayout.PropertyField(_minRadius, new GUIContent("Min Radius"));
            AddTooltip("Any object closer than the min radius isn't detected!");
            
            EditorGUILayout.PropertyField(_maxRadius, new GUIContent("Max Radius"));
            AddTooltip("Any object further away than the max radius isn't detected!");
            
            AddSpace(_defaultGUISpace);
            
            ShowDirectionField();
        
            AddSpace(_defaultGUISpace);
        }


        private void ShowSphericalVisionFields()
        {
            EditorGUILayout.PropertyField(_minRadius, new GUIContent("Min Radius"));
            AddTooltip("Any object closer than the min radius isn't detected!");
            
            EditorGUILayout.PropertyField(_maxRadius, new GUIContent("Max Radius"));
            AddTooltip("Any object further away than the max radius isn't detected!");
        }


        private void ShowConicalVisionFields()
        {
            EditorGUILayout.PropertyField(_fov, new GUIContent("Fov"));
            
            
            EditorGUILayout.PropertyField(_minRadius, new GUIContent("Min Radius"));
            AddTooltip("Any object closer than the min radius isn't detected!");
            
            EditorGUILayout.PropertyField(_maxRadius, new GUIContent("Max Radius"));
            AddTooltip("Any object further away than the max radius isn't detected!");
            
            AddSpace(_defaultGUISpace);
            
            ShowDirectionField();
        
            AddSpace(_defaultGUISpace);
        }


        private void ShowEventFields()
        {
            EditorGUILayout.PropertyField(_onObjDetected, true);
            AddTooltip("When an object is detected, this event will invoked!");

            if(!_visionController.GetNotifyObjExit) return;
            
            EditorGUILayout.PropertyField(_onObjExit, true);
            AddTooltip("When a detected object goes outside of the vision area, this event will invoked!");
        }


        private void ShowVisualizationFields()
        {
            EditorGUILayout.PropertyField(_visualize, new GUIContent("Visualize"));

            if (!_visionController.GetVisualize) return;

            AddSpace(_defaultGUISpace);

            EditorGUILayout.PropertyField(_normalColor, new GUIContent("Normal Color"));
            AddTooltip("The normal color of the visualization");
            
            
            EditorGUILayout.PropertyField(_detectedColor, new GUIContent("Detected Color"));
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
            serializedObject.ApplyModifiedProperties();
            _visionController.ValidateValues();
        }

        #endregion
    }
}


#endif