using UnityEngine;

[DisallowMultipleComponent]
public class AttackController : MonoBehaviour
{
    [Header("Aim / Camera")]
    [SerializeField] Transform cameraTransform;      // If left empty we’ll grab Camera.main

    [Header("Attack")]
    [SerializeField] float range = 3f;
    [SerializeField] int damage = 10;
    [SerializeField] float cooldown = 0.3f;
    [SerializeField] LayerMask hitMask = ~0;         // What we can hit (Default is fine)

    float _nextFire;

    void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Left click or Fire1
        if ((Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1")) && Time.time >= _nextFire)
        {
            _nextFire = Time.time + cooldown;
            TryHit();
        }
    }

    void TryHit()
    {
        if (!cameraTransform) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Walk up the hierarchy in case the collider is on a child
            var health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.Damage(damage);
                // (Optional) Debug so you see hits in the Console
                Debug.Log($"[Attack] Hit {health.name} for {damage}");
            }
        }
    }
}
