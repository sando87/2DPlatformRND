using UnityEngine;

namespace PahlBit
{
    public partial struct AnimStateNameHash
    {
        public static readonly int ExitDummy = Animator.StringToHash("ExitDummy");
        
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Run = Animator.StringToHash("Run");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int WakeUp = Animator.StringToHash("WakeUp");
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Respawn = Animator.StringToHash("Respawn");

        public static readonly int Skill = Animator.StringToHash("Skill");
        public static readonly int Dash = Animator.StringToHash("Dash");
        public static readonly int HitFlying = Animator.StringToHash("HitFlying");
        public static readonly int HitStrong = Animator.StringToHash("HitStrong");

        public static readonly int MeleeA = Animator.StringToHash("MeleeA");
        public static readonly int MeleeB = Animator.StringToHash("MeleeB");
        public static readonly int MeleeC = Animator.StringToHash("MeleeC");
        public static readonly int MeleeD = Animator.StringToHash("MeleeD");

        public static readonly int UpperIdle = Animator.StringToHash("UpperIdle");
        public static readonly int UpperAttack = Animator.StringToHash("UpperAttack");

    }
}