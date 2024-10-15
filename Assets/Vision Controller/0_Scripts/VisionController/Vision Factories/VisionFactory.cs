using UnityEngine;

namespace Vision_Controller
{
    [System.Serializable]
    public abstract class VisionFactory : ScriptableObject
    {
        [SerializeField] private VisionMode mode = VisionMode.CylindricalVision;
        public virtual VisionMode Mode() => mode;
        
        public abstract Vision CreateVision(Transform trans, VisionData data);
    }
}