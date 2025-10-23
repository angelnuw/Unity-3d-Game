using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Aim / Camera")]
    public Transform cameraTransform;   // auto-bound if empty
    public bool lockCursor = true;

    [Header("Attack")]
    public float range = 100f;
    public int damage = 10;
    public float cooldown = 0.25f;
    public LayerMask hitMask = ~0;      // set to Everything in Inspector

    CharacterController cc;
    Vector3 velocity;
    float yaw;
    float pitch;
    float nextAttackTime;

    Health selfHealth;
    Animator anim;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        selfHealth = GetComponent<Health>();
        anim = GetComponentInChildren<Animator>();

        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = cameraTransform ? cameraTransform.eulerAngles.x : 0f;
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Look();
        Move();
        UpdateAnimator();  // <<< drives Speed / Grounded

        // Simple test heal/damage keys (optional)
        if (selfHealth)
        {
            if (Input.GetKeyDown(KeyCode.H)) selfHealth.Damage(5);
            if (Input.GetKeyDown(KeyCode.J)) selfHealth.Heal(5);
        }

        // Left mouse attack (camera forward)
        if (Time.time >= nextAttackTime && Input.GetMouseButtonDown(0))
        {
            Attack();
            nextAttackTime = Time.time + cooldown;

            // optional: pulse Attack trigger if your Animator has it
            if (anim) anim.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.E) && cameraTransform)
        {
            Vector3 o = cameraTransform.position;
            Vector3 d = cameraTransform.forward;
            float maxDist = 3f;

            Debug.DrawRay(o, d * maxDist, Color.cyan, 0.25f);

            if (Physics.Raycast(o, d, out var hit, maxDist, ~0, QueryTriggerInteraction.Collide))
            {
                var swordStone = hit.collider.GetComponentInParent<InteractSwordStone>();
                if (swordStone != null)
                {
                    swordStone.PullSword(transform); // pass player so it can attach to your hand
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            FindObjectOfType<InteractSwordStone>()?.PullSword(transform);
        }
    }

    void Look()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (cameraTransform) cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void Move()
    {
        float hx = Input.GetAxisRaw("Horizontal");
        float vz = Input.GetAxisRaw("Vertical");

        // camera-relative move on ground plane
        Vector3 fwd = cameraTransform
            ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized
            : Vector3.forward;

        Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;

        Vector3 horiz = right * hx + fwd * vz;
        if (horiz.sqrMagnitude > 1f) horiz.Normalize();

        Vector3 motion = horiz * moveSpeed;

        // ground + jump
        if (cc.isGrounded)
        {
            if (velocity.y < 0f) velocity.y = -2f; // stick to ground
            if (Input.GetButtonDown("Jump"))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        motion.y = velocity.y;

        cc.Move(motion * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        if (!anim) return;

        // horizontal speed only (ignore falling)
        Vector3 hv = new Vector3(cc.velocity.x, 0f, cc.velocity.z);
        float speed = hv.magnitude;

        anim.SetFloat("Speed", speed);
        anim.SetBool("Grounded", cc.isGrounded);
    }

    void Attack()
    {
        if (!cameraTransform) return;

        Vector3 origin = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;

        // ignore our Player layer
        int mask = hitMask & ~LayerMask.GetMask("Player");

        Debug.DrawRay(origin, dir * range, Color.red, 0.25f);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, mask, QueryTriggerInteraction.Ignore))
        {
            var targetHealth = hit.collider.GetComponentInParent<Health>();
            if (targetHealth != null && targetHealth.gameObject != this.gameObject)
            {
                targetHealth.Damage(damage);
                Debug.Log($"[Attack] Hit {hit.collider.name} at {hit.distance:0.0}m, damage {damage}");

                // flash if available
                if (hit.collider.GetComponentInParent<HitFlash>() is HitFlash flash) flash.Flash();

                // (optional) damage numbers if you wired them
                // DamageNumber.Spawn(hit.point + Vector3.up * 0.35f, damage);
            }
            else
            {
                Debug.Log("[Attack] Hit something without Health.");
            }
        }
        else
        {
            Debug.Log("[Attack] Raycast hit nothing.");
        }
    }
}

