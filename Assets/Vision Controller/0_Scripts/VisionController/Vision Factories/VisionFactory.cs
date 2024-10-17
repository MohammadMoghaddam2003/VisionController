using UnityEngine;

namespace Vision_Controller
{
    [System.Serializable]
    public abstract class VisionFactory : ScriptableObject
    {
        // The vision mode determine how to calculate the vision!
        [SerializeField] private VisionMode mode = VisionMode.CylindricalVision;
        public virtual VisionMode Mode() => mode;
        
        public abstract Vision CreateVision(Transform trans, VisionData data);
    }
}