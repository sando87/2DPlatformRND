using UnityEngine;

namespace PahlBit
{
    public partial struct AnimStateNameHash
    {
        public static readonly int Idle = Animator.StringToHash("PlayerIdle");
        public static readonly int Run = Animator.StringToHash("PlayerRun");
        public static readonly int Jump = Animator.StringToHash("PlayerJump");
        public static readonly int Melee = Animator.StringToHash("PlayerMelee");
        public static readonly int Skill = Animator.StringToHash("PlayerSkill");
        public static readonly int Dash = Animator.StringToHash("PlayerDash");

        public static readonly int UpperIdle = Animator.StringToHash("UpperIdle");
    }
}