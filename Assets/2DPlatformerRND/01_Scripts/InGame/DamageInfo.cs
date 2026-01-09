using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public enum DamageType
    {
        Normal,
        Fire,
        Ice,
        Lightning,
        Poison,
    }
    public struct DamageInfo
    {
        public float Amount;
        public DamageType Type;
        public GameObject Attacker;
        public Vector3 HitPoint;       // 타격 위치
        public Vector3 HitDirection;   // 넉백 방향

        public DamageInfo(
            float amount,
            DamageType type = DamageType.Normal,
            GameObject attacker = null,
            Vector3 hitPoint = default,
            Vector3 hitDirection = default
        )
        {
            Amount = amount;
            Type = type;
            Attacker = attacker;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
        }
        public static implicit operator DamageInfo(float value)
        {
            return new DamageInfo(value);
        }
        public static implicit operator float(DamageInfo damage)
        {
            return damage.Amount;
        }
        public static DamageInfo operator +(DamageInfo a, DamageInfo b)
        {
            return new DamageInfo(
                a.Amount + b.Amount,
                a.Type,
                a.Attacker,
                a.HitPoint,
                a.HitDirection
            );
        }
        public static DamageInfo operator *(DamageInfo damage, float multiplier)
        {
            damage.Amount *= multiplier;
            return damage;
        }

        public static DamageInfo operator *(float multiplier, DamageInfo damage)
        {
            return damage * multiplier;
        }
        public static DamageInfo operator -(DamageInfo damage, float reduce)
        {
            damage.Amount = Mathf.Max(0, damage.Amount - reduce);
            return damage;
        }


    }
}
