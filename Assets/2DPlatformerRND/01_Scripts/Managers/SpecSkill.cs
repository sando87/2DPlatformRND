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
        public float Attack => BaseStats.Attack * mSpecPlayer.Option.AttackUp;
        public float ManaUse => BaseStats.ManaUse;
        public float Cooltime => BaseStats.Cooltime * mSpecPlayer.Option.CooltimeDown;
        public float ProjectileCount => BaseStats.ProjectileCount + mSpecPlayer.Option.ProjectileCountUp;
        public float ProjectileSpeed => BaseStats.ProjectileSpeed * mSpecPlayer.Option.ProjectileSpeedUp;
        public float AttackRange => BaseStats.AttackRange * mSpecPlayer.Option.AttackRangeUp;
        public float SplashRange => BaseStats.SplashRange * mSpecPlayer.Option.SplashRangeUp;
        public float Duration => BaseStats.Duration * mSpecPlayer.Option.DurationUp;
        public float Interval => BaseStats.Interval;

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

            BaseStats.Attack = ResourceData._Attack.GetValueByPoint(currentLevelIndex);
            BaseStats.ManaUse = ResourceData._ManaUse.GetValueByPoint(currentLevelIndex);
            BaseStats.Cooltime = ResourceData._Cooltime.GetValueByPoint(currentLevelIndex);
            BaseStats.ProjectileCount = ResourceData._ProjectileCount.GetValueByPoint(currentLevelIndex);
            BaseStats.ProjectileSpeed = ResourceData._ProjectileSpeed.GetValueByPoint(currentLevelIndex);
            BaseStats.AttackRange = ResourceData._AttackRange.GetValueByPoint(currentLevelIndex);
            BaseStats.SplashRange = ResourceData._SplashRange.GetValueByPoint(currentLevelIndex);
            BaseStats.Duration = ResourceData._Duration.GetValueByPoint(currentLevelIndex);
            BaseStats.Interval = ResourceData._Interval.GetValueByPoint(currentLevelIndex);
        }
    }
}