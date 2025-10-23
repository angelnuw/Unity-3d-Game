using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] float chaseRange = 30f;
    [SerializeField] float attackRange = 2f;

    NavMeshAgent agent;
    Transform target;     // player

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // reasonable defaults for a capsule 2m tall
        if (agent.height <= 0.01f) agent.height = 2f;
        if (agent.radius <= 0.01f) agent.radius = 0.5f;
    }

    IEnumerator Start()
    {
        // Wait one frame so GameManager can spawn/bind the player
        yield return null;

        var player = FindFirstObjectByType<PlayerController>();
        if (player) target = player.transform;
        else Debug.LogWarning("[EnemyChase] No PlayerController found in scene.");

        SnapToNavMesh();
    }

    void SnapToNavMesh()
    {
        // If the enemy starts slightly under/above the surface, put it on the mesh.
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                Debug.Log("[EnemyChase] Warped enemy onto NavMesh at " + hit.position);
            }
            else
            {
                Debug.LogWarning("[EnemyChase] Could not find NavMesh near enemy (radius 5).");
            }
        }
    }

    void Update()
    {
        if (!agent || !target) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= chaseRange)
        {
            agent.stoppingDistance = attackRange;
            agent.SetDestination(target.position);
        }

        // Optional debug:
        // Debug.DrawLine(transform.position + Vector3.up, agent.destination + Vector3.up, Color.cyan);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
