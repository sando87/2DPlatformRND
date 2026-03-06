using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using NaughtyAttributes.Test;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SpecSkill : SpecBase
    {
        public float ManaUse => BaseStats.ManaUse;
        public float Cooltime => BaseStats.Cooltime * mSpecPlayer.Option.CooltimeDown;
        public float ProjectileCount => BaseStats.ProjectileCount + mSpecPlayer.Option.ProjectileCountUp;
        public float ProjectileSpeed => BaseStats.ProjectileSpeed * mSpecPlayer.Option.ProjectileSpeedUp;
        public float AttackRange => BaseStats.AttackRange * mSpecPlayer.Option.AttackRangeUp;
        public float SplashRange => BaseStats.SplashRange * mSpecPlayer.Option.SplashRangeUp;
        public float Duration => BaseStats.Duration * mSpecPlayer.Option.DurationUp;
        public float Interval => BaseStats.Interval;
        public float StartDelay => BaseStats.StartDelay;

        public Percent PhyAttack => BaseStats.PhyAttack + mSpecPlayer.Option.PhyAttack;
        public Percent FireAttack => BaseStats.FireAttack + mSpecPlayer.Option.FireAttack;
        public Percent IceAttack => BaseStats.IceAttack + mSpecPlayer.Option.IceAttack;
        public Percent LightningAttack => BaseStats.LightningAttack + mSpecPlayer.Option.LightningAttack;

        public SkillSaveData SaveData { get; private set; } = null;
        public SkillResourceData ResourceData { get; private set; } = null;
        public SkillStats BaseStats { get; private set; } = null;

        private SpecPlayer mSpecPlayer = null;

        public void Init(int characterID, string resourceID)
        {
            ResourceData = SkillResourceTable.Instance.GetInfo(resourceID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            SaveData = userSaveData.Characters[characterID].Skills[resourceID];

            mSpecPlayer = this.ExGetBase().PlayerObj.Spec;

            UpdateBasicStat();
        }

        public void UpdateBasicStat()
        {
            int currentLevelIndex = SaveData == null ? 0 : SaveData.LevelIndex;

            BaseStats = new SkillStats();

            BaseStats.ManaUse = ResourceData._ManaUse.GetValueByPoint(currentLevelIndex);
            BaseStats.Cooltime = ResourceData._Cooltime.GetValueByPoint(currentLevelIndex);
            BaseStats.ProjectileCount = ResourceData._ProjectileCount.GetValueByPoint(currentLevelIndex);
            BaseStats.ProjectileSpeed = ResourceData._ProjectileSpeed.GetValueByPoint(currentLevelIndex);
            BaseStats.AttackRange = ResourceData._AttackRange.GetValueByPoint(currentLevelIndex);
            BaseStats.SplashRange = ResourceData._SplashRange.GetValueByPoint(currentLevelIndex);
            BaseStats.Duration = ResourceData._Duration.GetValueByPoint(currentLevelIndex);
            BaseStats.Interval = ResourceData._Interval.GetValueByPoint(currentLevelIndex);
            BaseStats.StartDelay = ResourceData._StartDelay.GetValueByPoint(currentLevelIndex);
            BaseStats.PhyAttack = (Percent)ResourceData._PhyAttack.GetValueByPoint(currentLevelIndex);
            BaseStats.FireAttack = (Percent)ResourceData._FireAttack.GetValueByPoint(currentLevelIndex);
            BaseStats.IceAttack = (Percent)ResourceData._IceAttack.GetValueByPoint(currentLevelIndex);
            BaseStats.LightningAttack = (Percent)ResourceData._LightningAttack.GetValueByPoint(currentLevelIndex);
        }

        public DamageInfo CalcCurrentDamages()
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.PhyDamage = mSpecPlayer.BaseAttack * PhyAttack;
            damageInfo.FireDamage = mSpecPlayer.BaseAttack * FireAttack;
            damageInfo.IceDamage = mSpecPlayer.BaseAttack * IceAttack;
            damageInfo.LightningDamage = mSpecPlayer.BaseAttack * LightningAttack;
            damageInfo.IsCritical = MyUtils.IsPercentHit((int)mSpecPlayer.Option.CriticalRate);
            damageInfo.CriticalAttackUp = mSpecPlayer.Option.CriticalAttack;
            return damageInfo;
        }
    }
}