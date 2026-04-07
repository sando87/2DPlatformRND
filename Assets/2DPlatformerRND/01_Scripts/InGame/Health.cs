using System.Collections;
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

        public float MaxHealth => mMaxCurrentHP;
        public float MaxMana => mMaxCurrentMana;
        public float MaxShield => mMaxCurrentShield;

        public float CurrentHP => mCurrentHP;
        public float CurrentMana => mCurrentMana;
        public float CurrentShield => mCurrentShield;

        public float CurrentTemputure { get; set; } = 0;

        public bool IsBurned { get; private set; }
        public bool IsFreezed { get; private set; }

        float mMaxCurrentHP = 10;
        float mMaxCurrentMana = 0;
        float mMaxCurrentShield = 0;

        [SerializeField, NaughtyAttributes.ReadOnly]
        float mCurrentHP = 10;
        [SerializeField, NaughtyAttributes.ReadOnly]
        float mCurrentMana = 0;
        [SerializeField, NaughtyAttributes.ReadOnly]
        float mCurrentShield = 0;

        public UnityEvent<DamagedResultInfo> OnDamaged = new UnityEvent<DamagedResultInfo>();
        public UnityEvent OnDied = new UnityEvent();

        BaseObject mBaseObj = null;
        SpecBase mSpec = null;

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mSpec = mBaseObj.Spec;
        }

        void Start()
        {
            InitHealth();

            StartCoroutine(CoProcessBurnOrFreez());
        }

        IEnumerator CoProcessBurnOrFreez()
        {
            while (true)
            {
                yield return new WaitUntil(() => CurrentTemputure < -10 || 10 < CurrentTemputure);

                if (CurrentTemputure > 10)
                {
                    while (CurrentTemputure > 0)
                    {
                        ApplyBurnEffect();
                        yield return newWaitForSeconds.Cache(1);
                        CurrentTemputure -= 4;
                    }
                    RemoveBurnEffect();
                }
                else if (CurrentTemputure < -10)
                {
                    while (CurrentTemputure < 0)
                    {
                        ApplySlowEffect();
                        yield return newWaitForSeconds.Cache(1);
                        CurrentTemputure += 4;
                    }
                    RemoveSlowEffect();
                }
            }
        }

        void ApplyBurnEffect()
        {
            if (IsDead)
                return;

            // 데미지 감소 처리
            float curTemp = CurrentTemputure;
            curTemp.ExSetMaximum(20);
            float tempRate = curTemp / 20f;
            float damage = tempRate * 10f;

            // 온도에 따른 이펙트 크기 감소 처리
            mBaseObj.Renderer.SetColor(StringHashes.ColorBurn, new Color(0.5f, 0.25f, 0));
            mBaseObj.Renderer.SetBurnRate(tempRate);

            DamagedResultInfo damagedResultInfo = new DamagedResultInfo();
            damagedResultInfo.MaxHealth = mMaxCurrentHP;
            damagedResultInfo.BeforeHealth = mCurrentHP;

            mCurrentHP -= damage;
            mCurrentHP.ExSetMinimum(0);

            damagedResultInfo.OriDamage = damage;
            damagedResultInfo.TotalDamage = damage;
            damagedResultInfo.ValidDamage = damage;
            damagedResultInfo.AfterHealth = mCurrentHP;

            OnDamaged.Invoke(damagedResultInfo);

            IsBurned = true;

            if (IsDead)
            {
                RemoveBurnEffect();
                OnDied.Invoke();
            }
        }
        void RemoveBurnEffect()
        {
            IsBurned = false;
            mBaseObj.Renderer.UnSetColor(StringHashes.ColorBurn);
            mBaseObj.Renderer.SetBurnRate(0);
        }

        void ApplySlowEffect()
        {
            if (IsDead)
                return;

            // 이속 감소 처리
            int buffID = mBaseObj.GetInstanceID();
            float moveSpeedUp = CurrentTemputure * 4;
            mBaseObj.Buffs.SetMoveSpeedBuff(buffID, new PercentUp(moveSpeedUp));

            mBaseObj.Renderer.SetColor(StringHashes.ColorFreez, Color.blue);

            IsFreezed = true;
        }
        void RemoveSlowEffect()
        {
            int buffID = mBaseObj.GetInstanceID();
            mBaseObj.Buffs.RemoveBuff(buffID);

            mBaseObj.Renderer.UnSetColor(StringHashes.ColorFreez);

            IsFreezed = false;
        }

        void InitHealth()
        {
            mMaxCurrentHP = mSpec.MaxHealth;
            mMaxCurrentMana = mSpec.MaxMana;
            mMaxCurrentShield = mSpec.MaxShield;

            mCurrentHP = mMaxCurrentHP;
            mCurrentMana = mMaxCurrentMana;
            mCurrentShield = mMaxCurrentShield;
        }

        DamagedResultInfo CalcHitResult(DamageInfo damageInfo)
        {
            DamagedResultInfo damageRetInfo = new DamagedResultInfo();
            damageRetInfo.MaxHealth = mMaxCurrentHP;
            damageRetInfo.BeforeHealth = mCurrentHP;
            damageRetInfo.OriDamage = damageInfo.PhyDamage + damageInfo.FireDamage + damageInfo.IceDamage + damageInfo.LightningDamage;

            // 물리 데미지 계산
            float phyDamage = damageInfo.IsCritical ? damageInfo.PhyDamage * damageInfo.CriticalAttackUp : damageInfo.PhyDamage;
            damageRetInfo.TotalDamage = phyDamage;

            // 파이어 데미지 계산
            float fireDamage = damageInfo.FireDamage * (Percent.One - mSpec.Option.FireResist);
            fireDamage.ExSetMinimum(0);
            damageRetInfo.TotalDamage += fireDamage;

            // 아이스 데미지 계산
            float iceDamage = damageInfo.IceDamage * (Percent.One - mSpec.Option.IceResist);
            iceDamage.ExSetMinimum(0);
            damageRetInfo.TotalDamage += iceDamage;

            // 라이트닝 데미지 계산
            float lightningDamage = damageInfo.LightningDamage * (Percent.One - mSpec.Option.LightningResist);
            lightningDamage.ExSetMinimum(0);
            damageRetInfo.TotalDamage += lightningDamage;

            return damageRetInfo;
        }

        public void GetDamaged(DamageInfo damage)
        {
            if (IsDead || damage <= 0) return;

            DamagedResultInfo damageRetInfo = CalcHitResult(damage);

            float fireTemputure = (damage.FireDamage / mMaxCurrentHP) * 100f;
            float iceTemputure = (damage.IceDamage / mMaxCurrentHP) * 100f;
            CurrentTemputure += (fireTemputure - iceTemputure);

            float remainDamage = damageRetInfo.TotalDamage;

            if (mCurrentShield > 0)
            {
                float usedShield = Mathf.Min(mCurrentShield, remainDamage);
                mCurrentShield -= usedShield;
                remainDamage -= usedShield;
                damageRetInfo.ValidDamage += usedShield;
            }

            if (remainDamage > 0)
            {
                remainDamage -= mSpec.PhyDefence;
                remainDamage.ExSetMinimum(0);
                damageRetInfo.ValidDamage += remainDamage;

                mCurrentHP -= remainDamage;
                mCurrentHP.ExSetMinimum(0);

                damageRetInfo.AfterHealth = mCurrentHP;
                OnDamaged.Invoke(damageRetInfo);

                if (IsDead)
                {
                    RemoveSlowEffect();
                    OnDied.Invoke();
                }
            }
        }
        public void GetDied()
        {
            if (IsDead) return;

            RemoveSlowEffect();
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
