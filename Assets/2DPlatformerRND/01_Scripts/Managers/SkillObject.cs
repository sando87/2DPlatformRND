using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillObject : MonoBehaviour
{
    [SerializeField]
    [Dropdown("IDList")]
    string _ID = "";
    List<string> IDList { get => SkillResourceTable.Instance.GetAllInfo().Select(info => info.SkillID).ToList(); }

    public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

    public SkillSaveData SaveData { get; private set; } = null;
    public SkillResourceData ResourceData { get; private set; } = null;
    public SkillStats BaseStats { get; private set; } = null;

    public long ResourceID => SkillResourceData.ToID(_ID);
    public bool IsEquipped { get => SaveData.IsEquipped; set => SaveData.IsEquipped = value; }
    public int PositionIndex { get => SaveData.PositionIndex; set { SaveData.PositionIndex = value; } }
    public int Level { get => SaveData.Level; set { SaveData.Level = value; } }

    protected BaseObject mBaseObj = null;
    protected PlayerUnitInput mInput = null;

    void Awake()
    {
        mBaseObj = this.ExGetBase();
        mInput = mBaseObj.Input;
        BaseStats = new SkillStats();
    }

    public void LoadSkillData()
    {
        int charID = CharRoot.CharacterID;
        SaveData = SaveFileManager<UserSaveData>.Load().Characters[charID].Skills[ResourceID];
        ResourceData = SkillResourceTable.Instance.GetInfo(ResourceID);
        UpdateBaseValue();
    }


    void UpdateBaseValue()
    {
        int currentLevelIndex = SaveData.LevelIndex;

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

    public virtual void OnEquipSkill()
    {
    }
    public virtual void UpdateSkill()
    {
    }
    public virtual void OnUnEquipSkill()
    {
    }

    public PlayerUnitInputType GetCurrentInputType()
    {
        switch (SaveData.PositionIndex)
        {
            case 0: return PlayerUnitInputType.SkillSlotA;
            case 1: return PlayerUnitInputType.SkillSlotB;
            case 2: return PlayerUnitInputType.SkillSlotC;
            case 3: return PlayerUnitInputType.SkillSlotD;
        }
        return PlayerUnitInputType.None;
    }

}
