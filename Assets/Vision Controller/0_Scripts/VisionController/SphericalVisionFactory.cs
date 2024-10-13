using UnityEngine;

namespace Vision_Controller
{
    [CreateAssetMenu(fileName = "Spherical Vision Factory", menuName = "Vision Factory/Spherical Vision Factory")]
    public class SphericalVisionFactory : VisionFactory
    {
        public override Vision CreateVision(Transform trans, VisionData data) => new SphericalVision(trans, data);
    }
}