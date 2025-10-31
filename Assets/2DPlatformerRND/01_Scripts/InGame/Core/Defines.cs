using UnityEngine;

namespace PahlBit
{
    /// <summary>
    /// 애니메이터 상태 이름을 해시로 미리 전환하여 사용
    /// </summary>
    public partial struct AnimStateNameHash
    {
        public int mHashValue;
        public AnimStateNameHash(int value)
        {
            mHashValue = value;
        }

        public static implicit operator int(AnimStateNameHash info) => info.mHashValue;
        public static implicit operator AnimStateNameHash(int val) => new AnimStateNameHash(val);

        public static int StringToHash(string stateName) { return Animator.StringToHash(stateName); }

        // examples
        // public static readonly int Idle = Animator.StringToHash("Idle");
        // public static readonly int Sit = Animator.StringToHash("Sit");
        // public static readonly int Walk = Animator.StringToHash("Walk");
        // public static readonly int Run = Animator.StringToHash("Run");
        // ...
    }
}