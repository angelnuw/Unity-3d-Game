using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageAnimRelay : MonoBehaviour
{
    [SerializeField] Animator anim;

    static readonly int HitHash = Animator.StringToHash("Hit");

    Health h;

    void Awake()
    {
        h = GetComponent<Health>();
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        if (h != null)
        {
            h.OnDamaged += HandleDamaged;
            h.OnDied += HandleDied;
        }
    }

    void OnDisable()
    {
        if (h != null)
        {
            h.OnDamaged -= HandleDamaged;
            h.OnDied -= HandleDied;
        }
    }

    void HandleDamaged(int amt)
    {
        if (anim) anim.SetTrigger(HitHash);
    }

    void HandleDied()
    {
        // Optional: disable player input, play a death pose, etc.
        // if (anim) anim.SetBool("Dead", true);
    }
}
