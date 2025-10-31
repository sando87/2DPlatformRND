using UnityEngine;

namespace PahlBit
{
    public partial struct AnimStateNameHash
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Sit = Animator.StringToHash("Sit");
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Run = Animator.StringToHash("Run");
    }
}