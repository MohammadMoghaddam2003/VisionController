using UnityEngine;

namespace Vision_Controller
{
    [CreateAssetMenu(fileName = "Conical Vision Factory", menuName = "Vision Factory/Conical Vision Factory")]
    public class ConicalVisionFactory : VisionFactory
    {
        public override Vision CreateVision(Transform trans, VisionData data) => new ConicalVision(trans, data);
    }
}