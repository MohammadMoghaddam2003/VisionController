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
        
        private SerializedProperty _data;
        private SerializedProperty _visionFactory;
        private SerializedProperty _direction;
        private SerializedProperty _center;
        private SerializedProperty _recheckTime;
        private SerializedProperty _targetLayer;
        private SerializedProperty _obstaclesLayer;
        private SerializedProperty _fov;
        private SerializedProperty _fos;
        private SerializedProperty _minRadius;
        private SerializedProperty _maxRadius;
        private SerializedProperty _maxObjDetection;
        private SerializedProperty _minHeight;
        private SerializedProperty _maxHeight;
        private SerializedProperty _notifyDetectedObjExit;
        private SerializedProperty _notifySensedObjExit;
        private SerializedProperty _blockCheck;
        private SerializedProperty _calculateSense;
        private SerializedProperty _onObjDetected;
        private SerializedProperty _onObjExit;
        private SerializedProperty _onObjSensed;
        private SerializedProperty _onSensedObjExit;
        private SerializedProperty _visualize;
        private SerializedProperty _normalColor;
        private SerializedProperty _detectedColor;
        private SerializedProperty _senseNormalColor;
        private SerializedProperty _sensedColor;


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
            _data = serializedObject.FindProperty("data");
            
            _visionFactory = _data.FindPropertyRelative("visionFactory");
            _direction = _data.FindPropertyRelative("direction");
            _center = _data.FindPropertyRelative("center");
            _recheckTime = _data.FindPropertyRelative("recheckTime");
            _maxObjDetection = _data.FindPropertyRelative("maxObjDetection");
            _targetLayer = _data.FindPropertyRelative("targetLayer");
            _obstaclesLayer = _data.FindPropertyRelative("obstaclesLayer");
            _fov = _data.FindPropertyRelative("fov");
            _fos = _data.FindPropertyRelative("fos");
            _minRadius = _data.FindPropertyRelative("minRadius");
            _maxRadius = _data.FindPropertyRelative("maxRadius");
            _minHeight = _data.FindPropertyRelative("minHeight");
            _maxHeight = _data.FindPropertyRelative("maxHeight");
            _notifyDetectedObjExit = _data.FindPropertyRelative("notifyDetectedObjExit");
            _notifySensedObjExit = _data.FindPropertyRelative("notifySensedObjExit");
            _blockCheck = _data.FindPropertyRelative("blockCheck");
            _calculateSense = _data.FindPropertyRelative("calculateSense");
            _onObjDetected = _data.FindPropertyRelative("onObjDetected");
            _onObjSensed = _data.FindPropertyRelative("onObjSensed");
            _onObjExit = _data.FindPropertyRelative("onObjExit");
            _onSensedObjExit = _data.FindPropertyRelative("onSensedObjExit");
            
            
            _visualize = serializedObject.FindProperty("visualize");
            _normalColor = serializedObject.FindProperty("normalColor");
            _detectedColor = serializedObject.FindProperty("detectedColor");
            _senseNormalColor = serializedObject.FindProperty("senseNormalColor");
            _sensedColor = serializedObject.FindProperty("sensedColor");
        }

        
        // Loads the images that used in the inspector from the directory!
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
        
        
        // Sets the icon to the target script
        private void SetIcon() => EditorGUIUtility.SetIconForObject(target, _icon);
        
        
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
            ShowVisualisationFields();


            ApplyModifiedFields();
        }
        


        /// <summary>
        /// Shows in the inspector the common fields that are used in all vision modes!
        /// </summary>
        private void ShowCommonFields()
        {
            EditorGUILayout.PropertyField(_visionFactory, true);
            AddTooltip("The vision factory determines which vision mode should be used!");
            
            EditorGUILayout.PropertyField(_targetLayer, true);
            AddTooltip("The layer of the objects which should be detected!");

            EditorGUILayout.PropertyField(_obstaclesLayer, true);
            AddTooltip("The layer of the objects which can block the targets so they don't detect!");

            EditorGUILayout.PropertyField(_recheckTime, true);
            AddTooltip("This specifies that every few seconds it should check if any objects is in the vision!");
            
            EditorGUILayout.PropertyField(_maxObjDetection, true);
            AddTooltip("Maximum objects that can be detected!");

            AddSpace(_defaultGUISpace / 3);

            EditorGUILayout.PropertyField(_center, true);
        }
        
        
        
        /// <summary>
        /// Shows in the inspector the specific fields of each vision mode!
        /// </summary>
        private void ShowVisionSettingFields()
        {
            AddSpace(_defaultGUISpace);
            EditorGUILayout.PropertyField(_notifyDetectedObjExit, true);
            AddTooltip("It will inform when a detected object goes outside the vision/sense field!");
            
            EditorGUILayout.PropertyField(_blockCheck, true);
            AddTooltip("Determines whether to calculate whether the target is behind something and blocked!");
            
            
            switch (_visionController.GetData.GetMode)
            {
                case VisionMode.CylindricalVision:
                {
                    ShowSenseToggleFields();
                    
                    AddSpace(_defaultGUISpace);
                    
                    ShowCylindricalVisionFields();
                    break;
                }

                case VisionMode.SphericalVision:
                {
                    AddSpace(_defaultGUISpace);

                    ShowSphericalVisionFields();
                    break;
                }

                case VisionMode.ConicalVision:
                {
                    ShowSenseToggleFields();
                    
                    AddSpace(_defaultGUISpace);
                    
                    ShowConicalVisionFields();
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }




            void ShowSenseToggleFields()
            {
                EditorGUILayout.PropertyField(_calculateSense, true);

                if (_visionController.GetData.GetCalculateSense)
                {
                    EditorGUILayout.PropertyField(_notifySensedObjExit, true);
                    AddTooltip("It will inform when a sensed object goes outside the vision/sense field!");
                }
            }
        }

        
        private void ShowCylindricalVisionFields()
        {
            EditorGUILayout.PropertyField(_fov, new GUIContent("Fov"));
            AddTooltip("Field of view");
            

            if (_visionController.GetData.GetCalculateSense)
            {
                EditorGUILayout.PropertyField(_fos, new GUIContent("Fos"));
                AddTooltip("Field of sense");
            }
            
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
            
            if (_visionController.GetData.GetCalculateSense)
            {
                EditorGUILayout.PropertyField(_fos, new GUIContent("Sense Field"));
            }
            
            EditorGUILayout.PropertyField(_minRadius, new GUIContent("Min Radius"));
            AddTooltip("Any object closer than the min radius isn't detected!");
            
            EditorGUILayout.PropertyField(_maxRadius, new GUIContent("Max Radius"));
            AddTooltip("Any object further away than the max radius isn't detected!");
            
            AddSpace(_defaultGUISpace);
            
            ShowDirectionField();
        
            AddSpace(_defaultGUISpace);
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

                int dir = -_visionController.GetData.GetDirection + 90;
                GUIUtility.RotateAroundPivot(dir, position.center);

                GUI.DrawTexture(position, _directionTexture, ScaleMode.ScaleToFit);
                
                GUI.matrix = originalMatrix;

                GUILayout.BeginHorizontal();
                
                int space = 40;
                if (dir > 10) space = 36;
                if (dir > 100) space = 33;
                
                AddSpace(space);
                GUILayout.Label($"{_visionController.GetData.GetDirection}°");
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
        
        
        private void ShowEventFields()
        {
            EditorGUILayout.PropertyField(_onObjDetected, true);
            AddTooltip("When an object is detected, this event will invoked!");

            if(!_visionController.GetData.GetNotifyDetectedObjExit) return;
            
            EditorGUILayout.PropertyField(_onObjExit, true);
            AddTooltip("When a detected object goes outside of the vision area, this event will invoked!");
            
            
            if(!_visionController.GetData.GetCalculateSense || _visionController.GetData.GetMode == VisionMode.SphericalVision) return;
            
            EditorGUILayout.PropertyField(_onObjSensed, true);
            AddTooltip("When an object is sensed, this event will invoked!");
            
            
            if(!_visionController.GetData.GetNotifySensedObjExit) return;
            
            EditorGUILayout.PropertyField(_onSensedObjExit, true);
            AddTooltip("When a sensed object goes outside of the sense field, this event will invoked!");
        }


        
        /// <summary>
        /// Shows in the inspector the fields of vision visualisation! 
        /// </summary>
        private void ShowVisualisationFields()
        {
            EditorGUILayout.PropertyField(_visualize, new GUIContent("Visualize"));

            if (!_visionController.GetVisualize) return;

            AddSpace(_defaultGUISpace);

            EditorGUILayout.PropertyField(_normalColor, new GUIContent("Normal Color"));
            AddTooltip("The normal color of the visualisation");
            
            
            EditorGUILayout.PropertyField(_detectedColor, new GUIContent("Detected Color"));
            AddTooltip("The color of the visualisation when something detected");
            
            
            if (!_visionController.GetData.GetCalculateSense || _visionController.GetData.GetMode == VisionMode.SphericalVision) return;

            EditorGUILayout.PropertyField(_senseNormalColor, new GUIContent("Sense Normal Color"));
            AddTooltip("The normal color of the sense field visualisation");
            
            
            EditorGUILayout.PropertyField(_sensedColor, new GUIContent("Sensed Color"));
            AddTooltip("The color of the sense field visualisation when something detected");
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
        
        
        
        /// <summary>
        /// Applies all fields changes in the serialized object! 
        /// </summary>
        private void ApplyModifiedFields()
        {
            serializedObject.ApplyModifiedProperties();
            _visionController.ValidateValues();
            _visionController.ResetVisualizationColors();
        }

        #endregion
    }
}


#endif