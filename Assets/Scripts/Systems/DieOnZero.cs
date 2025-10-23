using UnityEngine;
using UnityEngine.AI;

public class DieOnZero : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] float destroyDelay = 0.5f;
    bool done;

    void Awake()
    {
        if (!health) health = GetComponent<Health>();
    }

    void Update()
    {
        if (!done && health != null)
        {
            // Assumes Health exposes a 'Current' value. If not, add a public getter or event to Health.
            if (health.Current <= 0)
            {
                done = true;
                if (TryGetComponent(out NavMeshAgent agent)) agent.enabled = false;
                foreach (var col in GetComponentsInChildren<Collider>()) col.enabled = false;
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
