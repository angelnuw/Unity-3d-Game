using UnityEngine;

public class AutoGrounder : MonoBehaviour
{
    [Header("Grounding")]
    public LayerMask GroundMask = ~0; // which layers count as ground
    public float CastHeight = 5f;     // how high above we start the ray
    public float ExtraUp = 0.05f;     // tiny lift to avoid z-fighting

    /// <summary>Snap this object onto the ground directly below it.</summary>
    public void SnapToGround()
    {
        // Start a ray above the current position
        Vector3 origin = transform.position + Vector3.up * CastHeight;

        if (Physics.Raycast(origin, Vector3.down, out var hit, CastHeight * 2f, GroundMask, QueryTriggerInteraction.Ignore))
        {
            // If there's a CharacterController, respect its height (place bottom on ground)
            float halfHeight = 0f;
            var cc = GetComponent<CharacterController>();
            if (cc) halfHeight = cc.height * 0.5f;

            Vector3 pos = hit.point + Vector3.up * (halfHeight + ExtraUp);
            transform.position = pos;

            // Face +Z by default (adjust if your forward is opposite)
            // transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            Debug.Log($"[AutoGrounder] snapped to {pos} (ground: {hit.collider.name})");
        }
        else
        {
            Debug.LogWarning("[AutoGrounder] No ground hit!");
        }
    }
}
