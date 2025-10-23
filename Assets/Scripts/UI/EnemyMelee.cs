using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMelee : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] int damage = 10;
    [SerializeField] float attackRange = 1.9f;      // match your agent.stoppingDistance (~2)
    [SerializeField] float attackCooldown = 0.75f;  // seconds between hits

    [Header("Optional")]
    [SerializeField] bool faceTargetOnAttack = true;

    NavMeshAgent agent;
    Transform target;
    Health targetHealth;
    float nextAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    IEnumerator Start()
    {
        // Allow GameManager to spawn/bind the player
        yield return null;

        var player = FindFirstObjectByType<PlayerController>();
        if (player)
        {
            target = player.transform;
            targetHealth = player.GetComponent<Health>();
        }
        else
        {
            Debug.LogWarning("[EnemyMelee] No PlayerController found.");
        }
    }

    void Update()
    {
        if (!target || targetHealth == null || targetHealth.Current <= 0) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // Stop exactly at attack range (use same value on your NavMeshAgent.stoppingDistance)
        if (agent) agent.stoppingDistance = attackRange;

        if (dist <= attackRange && Time.time >= nextAttackTime)
        {
            if (faceTargetOnAttack)
            {
                Vector3 to = (target.position - transform.position);
                to.y = 0f;
                if (to.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.LookRotation(to.normalized);
            }

            // Deal damage
            targetHealth.Damage(damage);

            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
