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
    public SkillInfo SkillInfo { get; private set; }
    public SkillStats BaseStats { get => SkillInfo.BaseStats; }

    public static SkillObject Create(long skillResID)
    {
        SkillResourceData resData = SkillResourceTable.Instance.GetInfo(skillResID);
        SkillObject skillPrefab = Resources.Load<SkillObject>("Prefabs/Skills/" + resData.PrefabName);
        SkillObject skillObj = Instantiate(skillPrefab);
        skillObj.SkillInfo = new SkillInfo();
        skillObj.SkillInfo.InitSKillResourceData(resData);
        return skillObj;
    }
    public static SkillObject Create(SkillSaveData skillSaveData, Transform parent)
    {
        SkillObject skillObj = Create(skillSaveData.ResourceID);
        skillObj.SkillInfo.ApplySaveData(skillSaveData);
        skillObj.transform.SetParent(parent);
        skillObj.transform.localPosition = Vector3.zero;
        skillObj.transform.localRotation = Quaternion.identity;
        skillObj.transform.localScale = Vector3.one;
        return skillObj;
    }

    public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();


    protected BaseObject mBaseObj = null;
    protected PlayerUnitInput mInput = null;

    void Awake()
    {
        mBaseObj = this.ExGetBase();
        mInput = mBaseObj.Input;
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

}
