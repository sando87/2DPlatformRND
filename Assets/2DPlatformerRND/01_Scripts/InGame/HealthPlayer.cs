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
                Heal(mSpecPlayer.HealthRegen);
            }

            if (mSpecPlayer.ManaRegen > 0)
            {
                RestoreMana(mSpecPlayer.ManaRegen);
            }

            if (mSpecPlayer.ShieldRegen > 0)
            {
                RestoreShield(mSpecPlayer.ShieldRegen);
            }
        }

    }
}
