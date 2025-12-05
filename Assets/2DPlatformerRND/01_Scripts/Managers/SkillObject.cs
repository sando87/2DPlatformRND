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

        BaseStats.Attack = ResourceData.Attack + (ResourceData.AttackPerLv * currentLevelIndex);
        BaseStats.ManaUse = ResourceData.ManaUse + (ResourceData.ManaUsePerLv * currentLevelIndex);
        BaseStats.Cooltime = ResourceData.Cooltime - (ResourceData.CooltimeDownPerLv * currentLevelIndex);
        BaseStats.ProjectileCount = ResourceData.ProjectileCount + (ResourceData.ProjectileCountPerLv * currentLevelIndex);
        BaseStats.ProjectileSpeed = ResourceData.ProjectileSpeed;
        BaseStats.ProjectileDistance = ResourceData.ProjectileDistance;
        BaseStats.AttackRange = ResourceData.AttackRange + (ResourceData.AttackRangePerLv * currentLevelIndex);
        BaseStats.SplashRange = ResourceData.SplashRange + (ResourceData.SplashRangePerLv * currentLevelIndex);
        BaseStats.Duration = ResourceData.Duration + (ResourceData.DurationPerLv * currentLevelIndex);
        BaseStats.Interval = ResourceData.Interval + (ResourceData.IntervalPerLv * currentLevelIndex);
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
