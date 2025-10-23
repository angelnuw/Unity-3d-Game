using UnityEngine;
using System.Threading.Tasks;

namespace YourGameNamespace.Controllers
{
    public class AnimatorController : MonoBehaviour
    {
        public static JumpSetting Jump { get; set; }

        [SerializeField] private Animator anim;
        [SerializeField] private PlayerMovement player;
        [Space(2)]
        [SerializeField] private JumpSetting jump;
        [SerializeField] private JumpSetting jumpWeapon;

        private float InputX => Input.GetAxis("Horizontal");
        private float InputZ => Input.GetAxis("Vertical");

        private bool IsWalk => InputX != 0 || InputZ != 0;
        private bool IsRun => Input.GetKey(KeyCode.LeftShift) && IsWalk;
        private bool IsAgres => Input.GetKeyDown(KeyCode.I);
        private bool IsUp => Input.GetKeyDown(KeyCode.F);

        private bool IsAttack1 => Input.GetMouseButtonDown(0);
        private bool IsAttack2 => Input.GetMouseButtonDown(1);
        private bool IsAttack3 => Input.GetMouseButtonDown(2);

        private bool _isPuckUp = false;
        private bool _isAgres = true;
        private bool _isJump = false;

        private void Awake()
        {
            player.ChangeStartPlayJump += StartAnimJump;
            player.ChangeFly += FlyJump;
            Jump = jump;
        }

        private void Start()
        {
            anim.SetBool("IsAgres", _isAgres);
        }

        private void Update()
        {
            if (_isJump) return;
            if (IsWalk)
            {
                anim.SetBool("IsRun", false);
                anim.SetBool("IsWalk", true);
                anim.SetBool("IsRunNW", false);
                anim.SetBool("IsWalkNW", true);
            }
            if (IsRun)
            {
                anim.SetBool("IsRun", true);
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsRunNW", true);
                anim.SetBool("IsWalkNW", false);
            }

            if (!IsWalk && !IsRun)
            {
                anim.SetBool("IsRun", false);
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsRunNW", false);
                anim.SetBool("IsWalkNW", false);
            }
            if (IsAgres)
            {
                _isAgres = !_isAgres;
                anim.SetBool("IsAgres", _isAgres);
            }

            if (IsUp)
            {
                _isPuckUp = !_isPuckUp;
                if (_isPuckUp)
                {
                    anim.SetTrigger("IsWU");
                    _isAgres = true;
                    anim.SetBool("IsAgres", _isAgres);
                }
                else
                    anim.SetTrigger("IsWD");
            }

            if (_isPuckUp || !_isPuckUp)
            {
                // Этот код выполнится всегда, так как одно из условий всегда истинно.
            }

            {
                if (IsAttack1) anim.SetTrigger("Attack1");
                if (IsAttack2) anim.SetTrigger("Attack2");
                if (IsAttack3) anim.SetTrigger("Attack3");
            }
        }

        private void FlyJump(bool isFly)
        {
            _isJump = isFly;
            if (_isPuckUp)
                anim.SetBool("NoGround", isFly);
            else
                anim.SetBool("NoGroundNW", isFly);
        }

        private void StartAnimJump()
        {
            if (_isPuckUp)
            {
                anim.SetTrigger("Jumping");
                Jump = jumpWeapon;
            }
            else
            {
                anim.SetTrigger("JumpNW");
                Jump = jump;
            }
            PlayJumpAnim();
        }

        private async void PlayJumpAnim()
        {
            await NewSpeed(Jump.PercentPreCast, Jump.TimePreCast);
            await NewSpeed(Jump.PercentCast, Jump.TimeCast);
        }

        private async Task NewSpeed(float percent, float time)
        {
            float times = time;
            while (anim.GetCurrentAnimatorStateInfo(0).IsTag("Jump") && times >= 0)
            {
                times -= Time.deltaTime;
                await Task.Yield();
            }
            if (times <= 0) return;

            var newSpeedAnim = anim.GetCurrentAnimatorStateInfo(0).length * percent / times;
            anim.speed = newSpeedAnim;

            await Task.Delay((int)(times * 1000));
            anim.speed = 1;
        }
    }
}
