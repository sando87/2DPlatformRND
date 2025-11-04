using UnityEngine;

namespace PahlBit
{
    public partial struct AnimStateNameHash
    {
        public static readonly int Idle = Animator.StringToHash("PlayerIdle");
        public static readonly int Run = Animator.StringToHash("PlayerRun");
        public static readonly int Jump = Animator.StringToHash("PlayerJump");
        public static readonly int MeleeA = Animator.StringToHash("MeleeA");
        public static readonly int MeleeB = Animator.StringToHash("MeleeB");
        public static readonly int MeleeC = Animator.StringToHash("MeleeC");
        public static readonly int MeleeD = Animator.StringToHash("MeleeD");
        public static readonly int Skill = Animator.StringToHash("PlayerSkill");
        public static readonly int Dash = Animator.StringToHash("PlayerDash");

        public static readonly int UpperIdle = Animator.StringToHash("UpperIdle");

        public static readonly int Hit = Animator.StringToHash("Hit");
    }
}