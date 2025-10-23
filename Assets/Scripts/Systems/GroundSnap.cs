using UnityEngine;
using UnityEngine.AI;

public class GroundSnap : MonoBehaviour
{
    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] float castHeight = 5f;
    [SerializeField] float extraUp = 0.05f;
    [SerializeField] float navSampleRadius = 4f;

    void Start()
    {
        Vector3 pos = transform.position;

        // 1) Raycast down to geometry (balcony, floor, etc.)
        Vector3 origin = pos + Vector3.up * castHeight;
        if (Physics.Raycast(origin, Vector3.down, out var hit, castHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            pos = hit.point + Vector3.up * extraUp;
        }

        // 2) If there is a NavMesh, snap to the nearest valid spot near 'pos'
        if (TryGetComponent(out NavMeshAgent agent))
        {
            if (NavMesh.SamplePosition(pos, out var navHit, navSampleRadius, NavMesh.AllAreas))
            {
                // Warp so Agent height/grounding stays correct
                agent.Warp(navHit.position + Vector3.up * extraUp);
                return;
            }
            else
            {
                // Fall back to plain position if no navmesh found nearby
                agent.Warp(pos);
                return;
            }
        }

        // No agent ? just set transform
        transform.position = pos;
    }
}
