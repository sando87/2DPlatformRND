using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework;
using PahlBit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SkillObject : MonoBehaviour
{
    [SerializeField]
    [Dropdown("IDList")]
    long mSkillResID = 0;
    DropdownList<long> IDList()
    {
        DropdownList<long> rets = new DropdownList<long>();
        SkillResourceData[] resList = SkillResourceTable.Instance.GetAllInfo();
        foreach (SkillResourceData resData in resList)
            rets.Add(resData.SkillID, resData.ID);
        return rets;
    }

    [Button]
    [ShowIf(nameof(IsShowLearn))]
    void LearnSkill()
    {
        SkillController skillController = GetComponentInParent<SkillController>();
        skillController.LearnNewSkill(SkillID);
    }
    bool IsShowLearn() { return Application.isPlaying && !SkillInfo.IsLearned; }

    [Button]
    [ShowIf(nameof(IsShowLevelUp))]
    void LevelUpSkill()
    {
        SkillController skillController = GetComponentInParent<SkillController>();
        skillController.LevelupSkill(SkillID);
    }
    bool IsShowLevelUp() { return Application.isPlaying && SkillInfo.IsLearned; }

    [Button]
    [ShowIf(nameof(IsShowEquip))]
    void EquipSkill()
    {
        SkillController skillController = GetComponentInParent<SkillController>();
        int slotIdx = skillController.FindEmptySkillSlotIndex();
        if (slotIdx >= 0)
            skillController.EquipSkill(SkillID, slotIdx);
    }
    bool IsShowEquip() { return Application.isPlaying && SkillInfo.IsLearned && !SkillInfo.IsEquipped; }

    [Button]
    [ShowIf(nameof(IsShowUnEquip))]
    void UnEquipSkill()
    {
        SkillController skillController = GetComponentInParent<SkillController>();
        skillController.UnEquipSkill(SkillID, SkillInfo.PositionIndex);
    }
    bool IsShowUnEquip() { return Application.isPlaying && SkillInfo.IsLearned && SkillInfo.IsEquipped; }

    public SkillInfo SkillInfo { get; private set; }
    public SkillStats BaseStats { get => SkillInfo.BaseStats; }
    public long SkillID { get => SkillInfo.ResourceID; }

    [SerializeField]
    private SkillStats _BaseStats = null;

    public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

    protected BaseObject mBaseObj = null;
    protected PlayerUnitInput mInput = null;

    void Awake()
    {
        mBaseObj = this.ExGetBase();
        mInput = mBaseObj.Input;
    }

    public void InitSkillInfo()
    {
        UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
        int charID = CharRoot.CharacterID;
        var saveDataAllSkills = saveData.Characters[charID].Skills;
        if (!saveDataAllSkills.ContainsKey(mSkillResID))
        {
            saveDataAllSkills[mSkillResID] = new SkillSaveData(mSkillResID);
        }

        SkillSaveData skillSaveData = saveDataAllSkills[mSkillResID];
        SkillInfo = new SkillInfo();
        SkillInfo.ApplySaveData(skillSaveData);

        _BaseStats = SkillInfo.BaseStats;
    }

    public virtual bool IsCastable()
    {
        return true;
    }
    public virtual void StartCasting()
    {
    }
    public virtual void DoFire()
    {
    }
    public virtual void EndSkill()
    {
    }

    public virtual void OnLevelupSkill()
    {
        SkillInfo.Level++;
        SkillInfo.UpdateValue();
        _BaseStats = BaseStats;
    }
    public virtual void OnLearnSkill()
    {
        SkillInfo.IsLearned = true;
    }
    public virtual void OnEquipSkill(int slotIndex)
    {
        SkillInfo.IsEquipped = true;
        SkillInfo.PositionIndex = slotIndex;
    }
    public virtual void UpdateSkill()
    {
    }
    public virtual void OnUnEquipSkill()
    {
        SkillInfo.IsEquipped = false;
        SkillInfo.PositionIndex = -1;
    }

    public PlayerUnitInputType GetCurrentInputType()
    {
        if (SkillInfo.SaveData == null)
            return PlayerUnitInputType.None;

        switch (SkillInfo.PositionIndex)
        {
            case 0: return PlayerUnitInputType.SkillSlotA;
            case 1: return PlayerUnitInputType.SkillSlotB;
            case 2: return PlayerUnitInputType.SkillSlotC;
            case 3: return PlayerUnitInputType.SkillSlotD;
        }
        return PlayerUnitInputType.None;
    }

    protected void ApplyStatsToProjectile(ProjectileBase proj)
    {
        proj.Stats.MoveSpeed = BaseStats.ProjectileSpeed;
        proj.Stats.MaxDistance = BaseStats.ProjectileDistance;
        proj.Stats.SkillRange = BaseStats.AttackRange;
        proj.Stats.SplashRange = BaseStats.SplashRange;
        proj.Stats.Duration = BaseStats.Duration;
        proj.Stats.Interval = BaseStats.Interval;
    }

}
