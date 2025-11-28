using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
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

    public long ResourceID => ResourceData.ID;
    public bool IsEquipped { get => SaveData.IsEquipped; set => SaveData.IsEquipped = value; }
    public int PositionIndex { get => SaveData.PositionIndex; set { SaveData.PositionIndex = value; } }
    public int Level { get => SaveData.Level; set { SaveData.Level = value; } }

    protected BaseObject mBaseObj = null;

    void Awake()
    {
        mBaseObj = this.ExGetBase();
        BaseStats = new SkillStats();
    }

    public void Load(long skillResID)
    {
        int charID = CharRoot.CharacterID;
        SaveData = SaveFileManager<UserSaveData>.Load().Characters[charID].Skills[skillResID];
        ResourceData = SkillResourceTable.Instance.GetInfo(skillResID);
        UpdateBaseValue();
    }


    void UpdateBaseValue()
    {
        int currentLevelIndex = SaveData.LevelIndex;

        BaseStats.Attack = ResourceData.Attack + (ResourceData.AttackPerLv * currentLevelIndex);
        BaseStats.ManaUse = ResourceData.ManaUse + (ResourceData.ManaUsePerLv * currentLevelIndex);
        BaseStats.Cooltime = ResourceData.Cooltime - (ResourceData.CooltimeDownPerLv * currentLevelIndex);
        BaseStats.ProjectileCount = ResourceData.ProjectileCount + (ResourceData.ProjectileCountPerLv * currentLevelIndex);
        BaseStats.AttackRange = ResourceData.AttackRange + (ResourceData.AttackRangePerLv * currentLevelIndex);
        BaseStats.SplashRange = ResourceData.SplashRange + (ResourceData.SplashRangePerLv * currentLevelIndex);
        BaseStats.Duration = ResourceData.Duration + (ResourceData.DurationPerLv * currentLevelIndex);
        BaseStats.Interval = ResourceData.Interval + (ResourceData.IntervalPerLv * currentLevelIndex);
    }

    public virtual bool IsCastable()
    {
        return true;
    }
    public virtual void DoCast()
    {
    }

}
