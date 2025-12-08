using PahlBit;
using UnityEngine;

[System.Serializable]
public class SkillInfo
{
    public override string ToString()
    {
        return ResourceData == null ? "none" : ResourceData.SkillID;
    }

    public SkillSaveData SaveData { get; private set; } = null;
    public SkillResourceData ResourceData { get; private set; } = null;
    public SkillStats BaseStats { get; private set; } = null;

    public long ResourceID => ResourceData.ID;
    public bool IsEquipped { get => SaveData.IsEquipped; set => SaveData.IsEquipped = value; }
    public bool IsLearned { get => SaveData.IsLearned; set => SaveData.IsLearned = value; }
    public int PositionIndex { get => SaveData.PositionIndex; set { SaveData.PositionIndex = value; } }
    public int Level { get => SaveData.Level; set { SaveData.Level = value; } }

    public void ApplySaveData(SkillSaveData skillSaveData)
    {
        SaveData = skillSaveData;
        ResourceData = SkillResourceTable.Instance.GetInfo(skillSaveData.ResourceID);
        BaseStats = new SkillStats();
        UpdateValue();
    }

    public void UpdateValue()
    {
        int currentLevelIndex = SaveData == null ? 0 : SaveData.LevelIndex;

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
