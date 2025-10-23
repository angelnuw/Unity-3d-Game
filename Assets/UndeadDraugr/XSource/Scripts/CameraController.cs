using UnityEngine;

namespace YourGameNamespace.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float sensitivity = 2;
        [SerializeField] private float smoothness = 0.5f;
        [SerializeField] private float clampYMax = 60;
        [SerializeField] private float clampYMin = -25;
        [Space]
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float minDistance = 4;
        [SerializeField] private float maxDistance = 20;
        [SerializeField] private LayerMask obstacleLayer;  // Layer for obstacles

        private Vector3 _offset;
        private float _currentY;
        private float _currentDistance;  // Use a variable to store the current distance

        public static float CurrentX { get; private set; }

        void Start()
        {
            _offset = transform.position - target.position;
            _currentDistance = _offset.magnitude;  // Set initial distance
        }

        void LateUpdate()
        {
            CurrentX += Input.GetAxis("Mouse X") * sensitivity;

            _currentY -= Input.GetAxis("Mouse Y") * sensitivity;
            _currentY = Mathf.Clamp(_currentY, clampYMin, clampYMax);

            Quaternion rotation = Quaternion.Euler(_currentY, CurrentX, 0);
            Vector3 desiredPosition = target.position + (rotation * _offset.normalized * _currentDistance);

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            float zoomAmount = scroll * zoomSpeed;
            _currentDistance = Mathf.Clamp(_currentDistance - zoomAmount, minDistance, maxDistance);

            desiredPosition = target.position + (rotation * _offset.normalized * _currentDistance);

            // Cast a ray from the camera to the player
            RaycastHit hit;
            if (Physics.Linecast(target.position, desiredPosition, out hit, obstacleLayer))
            {
                // If the ray hits an obstacle, move the camera to the hit point
                desiredPosition = hit.point;
            }

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness);
            transform.LookAt(target);
        }
    }
}
