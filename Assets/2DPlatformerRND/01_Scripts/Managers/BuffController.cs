using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class BuffController : MonoBehaviour
    {
        public SpecOption TotalBuffOption { get; private set; } = new SpecOption();

        public void Init()
        {
        }

        public void UpdateState()
        {
        }

        public void ApplyBuff(BuffInfo buffInfo)
        {
            // TotalBuffOption.Add();
        }
    }

    public class BuffInfo
    {
        public int BuffID { get; set; } = -1;
        public float FireDamagePerSec { get; set; } = 0;
        public float Duration { get; set; } = 0;
    }
}