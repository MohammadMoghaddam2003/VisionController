using UnityEngine;

namespace Vision_Controller
{
    public class SimpleCameraMovement : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float speed = 5;
        [SerializeField] private Transform target;


        private Vector3 _distanceOffset;
        private Vector3 _refVelocity;

        #endregion


        #region Methods

        private void Start() => _distanceOffset = target.position - transform.position;
        
        private void FixedUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position - _distanceOffset, 
                ref _refVelocity, speed * Time.deltaTime);
        }

        #endregion
    }
}