using UnityEngine;

namespace Vision_Controller
{
    [CreateAssetMenu(fileName = "Cylindrical Vision Factory", menuName = "Vision Factory/Cylindrical Vision Factory")]
    public class CylindricalVisionFactory : VisionFactory
    {
        public override Vision CreateVision(Transform trans, VisionData data) => new CylindricalVision(trans, data);
    }
}