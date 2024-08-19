using UnityEngine;

namespace Vision_Controller
{
    [RequireComponent(typeof(Rigidbody))]
    public class Simple3DMovementController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float moveSpeed = 5;
        [SerializeField] private float rotateSpeed = 100;
        
        // Used for smooth movement
        [Tooltip("Used for smooth movement")]
        [SerializeField] private float smoothOffset = 10;



        private float _moveHorizontal;
        private float _moveVertical;
        private float _rotationAmount;
        private Rigidbody _rigidbody;

        #endregion
        

        #region Methods

        private void Awake() => ReceiveDependencies();

        private void ReceiveDependencies()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        
        
        private void Update()
        {
            #region Receive movement inputs

            if (Input.GetKey(KeyCode.W))
            {
                _moveVertical = Mathf.Clamp(_moveVertical + Mathf.Lerp(0, 1, smoothOffset * Time.deltaTime), -1, 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _moveVertical = Mathf.Clamp(_moveVertical - Mathf.Lerp(0, 1, smoothOffset * Time.deltaTime), -1, 1);
            }
            else
            {
                _moveVertical = Mathf.Clamp(Mathf.Lerp(_moveVertical, 0, smoothOffset * Time.deltaTime), -1, 1);
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                _moveHorizontal = Mathf.Clamp(_moveHorizontal - Mathf.Lerp(0, 1, smoothOffset * Time.deltaTime), -1, 1);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _moveHorizontal = Mathf.Clamp(_moveHorizontal + Mathf.Lerp(0, 1, smoothOffset * Time.deltaTime), -1, 1);
            }
            else
            {
                _moveHorizontal = Mathf.Clamp(Mathf.Lerp(_moveHorizontal, 0, smoothOffset * Time.deltaTime), -1, 1);
            }

            #endregion
            
            #region Receive rotation inputs

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _rotationAmount = Mathf.Clamp(_rotationAmount - Mathf.Lerp(0, 1, smoothOffset * Time.deltaTime), -1, 1);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _rotationAmount = Mathf.Clamp(_rotationAmount + Mathf.Lerp(0, 1, smoothOffset * Time.deltaTime), -1, 1);
            }
            else
            {
                _rotationAmount = Mathf.Clamp(Mathf.Lerp(_rotationAmount, 0, smoothOffset * Time.deltaTime), -1, 1);
            }

            #endregion
        }


        private void FixedUpdate()
        {
            Move();
            Rotate();
           
            
            
            
            void Move()
            {
                //Checking that the movement speed is halved when the movement is diagonal
                float speed = (_moveHorizontal != 0 && _moveVertical != 0) ? moveSpeed / 2 : moveSpeed;
                
                Vector3 pos = transform.position;
                Vector3 moveAmount = new Vector3(_moveHorizontal * speed, pos.y, _moveVertical * speed);
            
                _rigidbody.MovePosition(pos + (moveAmount * Time.deltaTime));
            }
        
            void Rotate() => 
                transform.Rotate(new Vector3(0, _rotationAmount, 0) * rotateSpeed * Time.deltaTime);
        }

        #endregion
    }
}

