using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public class Health : MonoBehaviour
    {
        public bool IsDead => mCurrentHP <= 0;

        public float HpRate => mMaxCurrentHP > 0 ? mCurrentHP / mMaxCurrentHP : 0;
        public float ManaRate => mMaxCurrentMana > 0 ? mCurrentMana / mMaxCurrentMana : 0;
        public float ShieldRate => mMaxCurrentShield > 0 ? mCurrentShield / mMaxCurrentShield : 0;

        public float CurrentHP => mCurrentHP;
        public float CurrentMana => mCurrentMana;
        public float CurrentShield => mCurrentShield;

        float mMaxCurrentHP = 10;
        float mMaxCurrentMana = 0;
        float mMaxCurrentShield = 0;

        [SerializeField, NaughtyAttributes.ReadOnly]
        float mCurrentHP = 10;
        [SerializeField, NaughtyAttributes.ReadOnly]
        float mCurrentMana = 0;
        [SerializeField, NaughtyAttributes.ReadOnly]
        float mCurrentShield = 0;

        public UnityEvent OnDamaged = new UnityEvent();
        public UnityEvent OnDied = new UnityEvent();

        public void InitHealth(float maxHp, float maxMana, float maxShield)
        {
            mMaxCurrentHP = maxHp;
            mMaxCurrentMana = maxMana;
            mMaxCurrentShield = maxShield;

            mCurrentHP = mMaxCurrentHP;
            mCurrentMana = mMaxCurrentMana;
            mCurrentShield = mMaxCurrentShield;
        }

        public void GetDamaged(DamageInfo damage)
        {
            if (IsDead || damage <= 0) return;

            float remainDamage = damage;

            if (mCurrentShield > 0)
            {
                float usedShield = Mathf.Min(mCurrentShield, remainDamage);
                mCurrentShield -= usedShield;
                remainDamage -= usedShield;
            }

            if (remainDamage > 0)
            {
                mCurrentHP -= remainDamage;

                if (mCurrentHP <= 0)
                {
                    mCurrentHP = 0;
                    OnDied.Invoke();
                }
                else
                {
                    OnDamaged.Invoke();
                }
            }
        }
        public void GetDied()
        {
            if (IsDead) return;
            mCurrentHP = 0;
            OnDied.Invoke();
        }
        public void Heal(float amount)
        {
            if (IsDead || amount <= 0) return;
            mCurrentHP += amount;
            mCurrentHP.ExSetMaximum(mMaxCurrentHP);
        }
        public void UseMana(float amount)
        {
            mCurrentMana -= amount;
            mCurrentMana.ExSetMinimum(0);
        }
        public void RestoreMana(float amount)
        {
            mCurrentMana += amount;
            mCurrentMana.ExSetMaximum(mMaxCurrentMana);
        }
        public void RestoreShield(float amount)
        {
            mCurrentShield += amount;
            mCurrentShield.ExSetMaximum(mMaxCurrentShield);
        }
    }
}
