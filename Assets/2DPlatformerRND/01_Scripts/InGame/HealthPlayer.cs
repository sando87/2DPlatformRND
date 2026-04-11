using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public class HealthPlayer : Health
    {
        SpecPlayer mSpecPlayer = null;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            mSpecPlayer = mSpec as SpecPlayer;

            this.ExRepeatCoroutine(1, DoRegenStats);
        }

        void DoRegenStats()
        {
            if (mSpecPlayer.HealthRegen > 0)
            {
                int hpRegen = mSpecPlayer.HealthRegen.ToInt();
                mCurrentHP += hpRegen;
                mCurrentHP.ExSetMaximum(mMaxCurrentHP);
            }

            if (mSpecPlayer.ManaRegen > 0)
            {
                int manaRegen = mSpecPlayer.ManaRegen.ToInt();
                mCurrentMana += manaRegen;
                mCurrentMana.ExSetMaximum(mMaxCurrentMana);
            }

            if (mSpecPlayer.ShieldRegen > 0)
            {
                int shieldRegen = mSpecPlayer.ShieldRegen.ToInt();
                mCurrentShield += shieldRegen;
                mCurrentShield.ExSetMaximum(mMaxCurrentShield);
            }
        }

    }
}
