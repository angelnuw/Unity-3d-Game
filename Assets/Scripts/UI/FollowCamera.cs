using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Camera offset (relative to player)")]
    public Vector3 localOffset = new Vector3(0f, 1.6f, -3f);

    [Tooltip("Zero-out local rotation so PlayerController can drive look.")]
    public bool zeroLocalRotation = true;

    void Start()
    {
        // Find the player that GameManager spawned
        PlayerController pc;
#if UNITY_2023_1_OR_NEWER
        pc = FindFirstObjectByType<PlayerController>();
#else
        pc = FindObjectOfType<PlayerController>();
#endif
        if (!pc)
        {
            Debug.LogWarning("[CameraFollowBinder] No PlayerController found at Start.");
            return;
        }

        // Parent this camera to the player and position it
        transform.SetParent(pc.transform);
        transform.localPosition = localOffset;
        if (zeroLocalRotation) transform.localRotation = Quaternion.identity;

        // Tell PlayerController to use THIS camera for look
        pc.cameraTransform = transform;
    }
}
