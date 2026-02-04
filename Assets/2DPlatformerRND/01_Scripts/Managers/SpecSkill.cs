using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SpecSkill : SpecBase
    {
        public SkillSaveData SaveData { get; private set; } = null;
        public SkillResourceData ResourceData { get; private set; } = null;
        [field: SerializeField]
        public SkillStats BaseStats { get; private set; } = null;

        public void Init(int characterID, string resourceID)
        {
            ResourceData = SkillResourceTable.Instance.GetInfo(resourceID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            SaveData = userSaveData.Characters[characterID].Skills[resourceID];

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
            BaseStats.ProjectileDistance = ResourceData._ProjectileDistance.GetValueByPoint(currentLevelIndex);
            BaseStats.AttackRange = ResourceData._AttackRange.GetValueByPoint(currentLevelIndex);
            BaseStats.SplashRange = ResourceData._SplashRange.GetValueByPoint(currentLevelIndex);
            BaseStats.Duration = ResourceData._Duration.GetValueByPoint(currentLevelIndex);
            BaseStats.Interval = ResourceData._Interval.GetValueByPoint(currentLevelIndex);
        }
    }
}