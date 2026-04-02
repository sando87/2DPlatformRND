using System.Collections.Generic;
using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ItemInfo
{
    public SpecOption _Option = null;

    public override string ToString()
    {
        return SaveData == null ? "none" : SaveData.InstanceID;
    }

    public ItemSaveData SaveData { get; private set; } = null;
    public ItemResourceData ResourceData { get; private set; } = null;
    public SpecOption Option { get; private set; } = null;
    public Dictionary<string, string> DisplayInfo { get; private set; } = new Dictionary<string, string>();

    public string InstanceID => SaveData.InstanceID;
    public long ResourceID => ResourceData.ID;
    public bool IsEquipped { get => SaveData.IsEquipped; set => SaveData.IsEquipped = value; }
    public int Count { get => SaveData.Count; set => SaveData.Count = value; }
    public int PositionIndex { get => SaveData.PositionIndex; set { SaveData.PositionIndex = value; } }
    public int Level { get => SaveData.Level; set { SaveData.Level = value; UpdateOption(); } }

    public void InitRandomItem()
    {
        ResourceData = ItemResourceTable.Instance.GetRandomItem();

        SaveData = new ItemSaveData();
        SaveData.InstanceID = System.Guid.NewGuid().ToString();
        SaveData.ResourceID = ResourceData.ID;
        SaveData.IsEquipped = false;
        SaveData.Level = 1;
        SaveData.Count = 1;
        SaveData.PositionIndex = -1;

        UpdateOption();
    }
    public void LoadItem(ItemSaveData data)
    {
        SaveData = data;
        ResourceData = ItemResourceTable.Instance.GetInfo(SaveData.ResourceID);
        UpdateOption();
    }
    public void UpdateOption()
    {
        Option = new SpecOption();
        int point = SaveData.LevelIndex;
        System.Random ran = new System.Random(SaveData.RandomSeed);

        Option.HealthUp = (PercentUp)ResourceData._HealthUp.GetValueByPoint(point);
        Option.HealthRegen = ResourceData._HealthRegen.GetValue();
        Option.ManaUp = (PercentUp)ResourceData._ManaUp.GetValueByPoint(point);
        Option.ManaRegen = ResourceData._ManaRegen.GetValue();
        Option.BaseAttackAdd = ResourceData._BaseAttackAdd.GetValueByPoint(point);
        Option.PhyAttack = (Percent)ResourceData._PhyAttack.GetValueByPoint(point);
        Option.FireAttack = (Percent)ResourceData._FireAttack.GetValueByPoint(point);
        Option.IceAttack = (Percent)ResourceData._IceAttack.GetValueByPoint(point);
        Option.LightningAttack = (Percent)ResourceData._LightningAttack.GetValueByPoint(point);
        Option.DefenceUp = (PercentUp)ResourceData._DefenceUp.GetValueByPoint(point);
        Option.MoveSpeedUp = (PercentUp)ResourceData._MoveSpeedUp.GetValue();
        Option.AttackSpeedUp = (PercentUp)ResourceData._AttackSpeedUp.GetValue();
        Option.CooltimeDown = (PercentUp)ResourceData._CooltimeDown.GetValue();
        Option.ShieldAdd = ResourceData._ShieldAdd.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ShieldRegen = ResourceData._ShieldRegen.GetValue();
        Option.CriticalRate = (PercentUp)ResourceData._CriticalRate.GetValue();
        Option.CriticalAttack = (PercentUp)ResourceData._CriticalAttack.GetValue();
        Option.ProjectileCountUp = ResourceData._ProjectileCountUp.GetValue();
        Option.ProjectileSpeedUp = (PercentUp)ResourceData._ProjectileSpeedUp.GetValue();
        Option.AttackRangeUp = (PercentUp)ResourceData._AttackRangeUp.GetValue();
        Option.SplashRangeUp = (PercentUp)ResourceData._SplashRangeUp.GetValue();
        Option.DurationUp = (PercentUp)ResourceData._DurationUp.GetValue();
        Option.FireResist = (Percent)ResourceData._FireResist.GetValue();
        Option.IceResist = (Percent)ResourceData._IceResist.GetValue();
        Option.LightningResist = (Percent)ResourceData._LightningResist.GetValue();
        Option.PosionResist = (Percent)ResourceData._PosionResist.GetValue();

        UpdateDisplayInfo();
    }

    public void UpdateDisplayInfo()
    {
        DisplayInfo.Clear();
        if (Option == null)
            return;

        List<FieldData> fields = ReflectionFieldExtractor.GetFields(Option);
        foreach (var field in fields)
        {
            if (field.Value.Equals("0") || field.Value.Equals("0%"))
                continue;

            DisplayInfo[field.Name] = field.Value;
        }
    }
}
