using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public class HealthBoss : Health
    {
        EnemyBossBase mBoss = null;

        protected override void Awake()
        {
            base.Awake();

            mBoss = mBaseObj.GetComponent<EnemyBossBase>();
        }

        protected override void Start()
        {
            base.Start();

            HPBarUIPlayer hpUIBar = mBaseObj.GetComponentInChildren<HPBarUIPlayer>();
            if (hpUIBar != null)
            {
                hpUIBar.SetHealthStatus(this);
            }

            this.ExRepeatCoroutine(1, DoRegenStats);
        }

        void DoRegenStats()
        {
            if (mBoss.Spec.HealthRegen > 0)
            {
                Heal(mBoss.Spec.HealthRegen);
            }

            float manaRegen = mBoss.ManaRegen + mBoss.Spec.ManaRegen;
            if (manaRegen > 0)
            {
                RestoreMana(manaRegen);
            }

            if (mBoss.Spec.ShieldRegen > 0)
            {
                RestoreShield(mBoss.Spec.ShieldRegen);
            }
        }

    }
}
