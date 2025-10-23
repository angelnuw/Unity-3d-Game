using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform eyes;          // assign your Eyes child
    [SerializeField] private NavMeshAgent agent;      // auto-fills on Start if left null
    [SerializeField] private Transform target;        // player

    [Header("Behavior")]
    [SerializeField] private float detectRadius = 12f;
    [SerializeField] private float loseRadius = 16f;
    [SerializeField] private float repathInterval = 0.15f;
    [SerializeField] private LayerMask losMask = ~0;  // what can block vision; leave default for now

    private float _repathTimer;
    private bool _hasLOS;

    public void SetTarget(Transform t) => target = t;
    public void SetEyes(Transform e) => eyes = e;

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!eyes)
        {
            // Fallback: try to find child named "Eyes"
            var t = transform.Find("Eyes");
            if (t) eyes = t;
        }
    }

    private void Start()
    {
        if (!target)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    private void Update()
    {
        if (!agent || !target) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // Simple detection with hysteresis (avoid jitter)
        if (dist <= detectRadius) _hasLOS = HasLineOfSight();
        else if (dist >= loseRadius) _hasLOS = false;

        // Repath on a timer when we should chase
        _repathTimer -= Time.deltaTime;
        if (_hasLOS && _repathTimer <= 0f)
        {
            agent.SetDestination(target.position);
            _repathTimer = repathInterval;
        }

        // Optional: face movement direction smoothly
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            var look = agent.velocity.normalized;
            look.y = 0;
            if (look.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(look),
                    Time.deltaTime * 10f
                );
        }
    }

    private bool HasLineOfSight()
    {
        if (!eyes) return true; // no eyes set → assume yes
        Vector3 dir = (target.position + Vector3.up * 1.0f) - eyes.position;
        if (Physics.Raycast(eyes.position, dir.normalized, out var hit, Mathf.Infinity, losMask, QueryTriggerInteraction.Ignore))
        {
            // We “see” the player if the first hit is the player or a child of the player
            return hit.collider.GetComponentInParent<PlayerController>() != null;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRadius);
    }
#endif
}
