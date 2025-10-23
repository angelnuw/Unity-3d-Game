using UnityEngine;

namespace YourGameNamespace.Controllers
{
    public class JumpSetting : MonoBehaviour
    {
        private enum JumpState
        {
            Weapon,
            NoWeapon
        }

        [SerializeField] private JumpState state;
        [SerializeField] private float timePreCast;
        [SerializeField] private float timeCast;
        [SerializeField] private float timePostCast;
        [SerializeField] private float percentPreCast;
        [SerializeField] private float percentCast;

        public float TimePreCast => timePreCast;
        public float TimeCast => timeCast;
        public float TimePostCast => timePostCast;
        public float PercentPreCast => percentPreCast;
        public float PercentCast => percentCast;
    }
}
