using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public struct DamagedResultInfo
    {
        public float OriDamage;
        public float ValidDamage;

        public float BeforeHealth;
        public float AfterHealth;
        public float MaxHealth;

        public float DeltaHealth => AfterHealth - BeforeHealth;
        public float CurrentHealthRate => AfterHealth / MaxHealth;
    }
}
