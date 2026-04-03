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

public class SkillBase : MonoBehaviour
{
    [SerializeField] Sprite _Icon = null;
    public Sprite Icon => _Icon;
    
    [SerializeField]
    [Dropdown(nameof(IDList))]
    string _ResourceID = "";
    public string ResourceID => _ResourceID;
    List<string> IDList { get => SkillResourceTable.Instance.GetAllInfo().Select(info => info.SkillID).ToList(); }

    public SkillController Controller { get => GetComponentInParent<SkillController>(); }

    public bool IsEquipped => mSkillSaveData.IsEquipped;
    public bool IsLearned => mSkillSaveData != null && mSkillSaveData.IsLearned;
    public int PositionIndex => mSkillSaveData.PositionIndex;
    public int Level => mSkillSaveData.Level;
    public bool IsCooltime => Time.time - mCooltime < Spec.Cooltime;

    protected BaseObject mBaseObj = null;
    protected PlayerUnitInput mInput = null;

    private SkillSaveData mSkillSaveData = null;
    private float mCooltime = 0;

    public SpecSkill Spec { get; private set; } = null;
    protected void StartCooltime() { mCooltime = Time.time; }

    void Awake()
    {
        mBaseObj = this.ExGetBase();
        mInput = mBaseObj.Input;
    }

    public void InitSkillInfo(int characterID)
    {
        UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
        var saveDataAllSkills = saveData.Characters[characterID].Skills;
        if (!saveDataAllSkills.ContainsKey(_ResourceID))
        {
            saveDataAllSkills[_ResourceID] = new SkillSaveData(_ResourceID);
        }

        mSkillSaveData = saveDataAllSkills[_ResourceID];

        Spec = GetComponentInChildren<SpecSkill>();
        Spec.Init(characterID, _ResourceID);
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
        mSkillSaveData.Level++;
        Spec.UpdateBasicStat();
        GameSystem.DoSave_UserSaveData();
    }
    public virtual void OnLearnedSkill()
    {
        mSkillSaveData.IsLearned = true;
        GameSystem.DoSave_UserSaveData();
    }
    public virtual void OnEquipedSkill(int slotIndex)
    {
        mSkillSaveData.IsEquipped = true;
        mSkillSaveData.PositionIndex = slotIndex;
        GameSystem.DoSave_UserSaveData();
    }
    public virtual void UpdateSkill()
    {
    }
    public virtual void OnUnEquipedSkill()
    {
        mSkillSaveData.IsEquipped = false;
        mSkillSaveData.PositionIndex = -1;
        GameSystem.DoSave_UserSaveData();
    }

    public PlayerUnitInputType GetCurrentInputType()
    {
        if (!IsLearned || !IsEquipped)
            return PlayerUnitInputType.None;

        switch (PositionIndex)
        {
            case 0: return PlayerUnitInputType.SkillSlotA;
            case 1: return PlayerUnitInputType.SkillSlotB;
            case 2: return PlayerUnitInputType.SkillSlotC;
            case 3: return PlayerUnitInputType.SkillSlotD;
        }
        return PlayerUnitInputType.None;
    }

    protected void ApplySkillStatsToProjectile(ProjectileBase proj)
    {
        proj.Stats.MoveSpeed = Spec.ProjectileSpeed;
        proj.Stats.FireAngle = 0;
        proj.Stats.AttackRange = Spec.AttackRange;
        proj.Stats.SplashRange = Spec.SplashRange;
        proj.Stats.Duration = Spec.Duration;
        proj.Stats.Interval = Spec.Interval;
        proj.Stats.StartDelay = Spec.StartDelay;
    }
}
