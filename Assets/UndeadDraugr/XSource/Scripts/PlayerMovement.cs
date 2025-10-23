using UnityEngine;

namespace YourGameNamespace.Controllers
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float speedRun;
        [SerializeField] private float smoothness = 100;

        [SerializeField] private float gravityVelocity;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float speedRotate;
        [Space]
        [SerializeField] private float radiysCheck = 0.1f;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float dist;

        [SerializeField] private Transform viewCharacter;
        [SerializeField] private LayerMask layerChack;

        private float _gravity;
        private float _jumpTimeCast;
        private float _jumpTimePreCast;
        private float _jumpTimePostCast;
        private bool _isJump = false;
        private bool _isJumpPreCast = false;

        private CharacterController _character;

        public event System.Action ChangeStartPlayJump;
        public event System.Action<bool> ChangeFly;

        private Vector3 Movement => transform.TransformDirection(new Vector3(InputX * Speed, InputY, InputZ * Speed));

        private float Speed => (Input.GetKey(KeyCode.LeftShift) ? speedRun : speed) * Time.deltaTime;
        private float InputX => Input.GetAxis("Horizontal");
        private float InputZ => Input.GetAxis("Vertical");
        private float InputY => Jump(Input.GetButton("Jump"), IsGrounded());

        public bool IsGrounded()
        {
            return Physics.Raycast(transform.position + transform.TransformDirection(offset + Vector3.right * dist), Vector3.down, radiysCheck) ||
                   Physics.Raycast(transform.position + transform.TransformDirection(offset - Vector3.right * dist), Vector3.down, radiysCheck) ||
                   Physics.Raycast(transform.position + transform.TransformDirection(offset + Vector3.forward * dist), Vector3.down, radiysCheck) ||
                   Physics.Raycast(transform.position + transform.TransformDirection(offset - Vector3.forward * dist), Vector3.down, radiysCheck);
        }

        private void Awake()
        {
            _character = GetComponent<CharacterController>();
        }

        private void Update()
        {
            _character.Move(Movement);

            if (InputX != 0 || InputZ != 0)
            {
                Quaternion targetRotation = Quaternion.Euler(0, CameraController.CurrentX, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothness);

                Vector3 localMovementDirection = new Vector3(InputX, 0, InputZ).normalized;
                Vector3 rotatedMovementDirection = Quaternion.Euler(0, CameraController.CurrentX, 0) * localMovementDirection;
                Quaternion look = Quaternion.LookRotation(rotatedMovementDirection);
                viewCharacter.rotation = Quaternion.Lerp(viewCharacter.rotation, look, speedRotate * Time.deltaTime);
            }
        }

        private float Jump(bool isJumpClick, bool isGroundCheck)
        {
            if (_isJumpPreCast)
            {
                if (_jumpTimePreCast >= 0)
                {
                    _jumpTimePreCast -= Time.deltaTime;
                    return 0;
                }
                else
                {
                    _isJumpPreCast = false;
                    _isJump = true;
                    return 0;
                }
            }

            if (_isJump)
            {
                if (_jumpTimeCast >= 0)
                {
                    _jumpTimeCast -= Time.deltaTime;
                    return _jumpTimeCast * jumpSpeed * Time.deltaTime;
                }
                else
                {
                    ResetJumpData();
                    _isJump = false;
                    return 0;
                }
            }

            if (_jumpTimePostCast >= 0)
                _jumpTimePostCast -= Time.deltaTime;

            else if (isJumpClick && isGroundCheck && _jumpTimePostCast <= 0)
            {
                ChangeStartPlayJump();
                _isJumpPreCast = true;
                ResetJumpData();
                _jumpTimePostCast = AnimatorController.Jump.TimePostCast;
            }

            ChangeFly(!isGroundCheck);
            Debug.Log(isGroundCheck);

            if (isGroundCheck)
            {
                ResetJumpData();
                return 0;
            }
            else
            {
                _gravity += gravityVelocity * Time.deltaTime * Time.deltaTime;
                return -_gravity;
            }
        }

        private void ResetJumpData()
        {
            _gravity = 0;
            _jumpTimeCast = AnimatorController.Jump.TimeCast / 2;
            _jumpTimePreCast = AnimatorController.Jump.TimePreCast;
        }

        private void OnDrawGizmos()
        {
            Vector3 spherePosition = transform.position + transform.TransformDirection(offset) - new Vector3(0f, radiysCheck, 0f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + transform.TransformDirection(offset + Vector3.right * dist), transform.position + transform.TransformDirection(offset + Vector3.right * dist) - Vector3.up * radiysCheck);
            Gizmos.DrawLine(transform.position + transform.TransformDirection(offset - Vector3.right * dist), transform.position + transform.TransformDirection(offset - Vector3.right * dist) - Vector3.up * radiysCheck);
            Gizmos.DrawLine(transform.position + transform.TransformDirection(offset + Vector3.forward * dist), transform.position + transform.TransformDirection(offset + Vector3.forward * dist) - Vector3.up * radiysCheck);
            Gizmos.DrawLine(transform.position + transform.TransformDirection(offset - Vector3.forward * dist), transform.position + transform.TransformDirection(offset - Vector3.forward * dist) - Vector3.up * radiysCheck);
        }
    }
}
