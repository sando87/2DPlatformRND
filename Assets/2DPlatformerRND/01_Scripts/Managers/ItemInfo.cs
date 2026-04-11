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
    public string Name => ResourceData.DisplayName;
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

        Option.HealthUp = (PercentUp)ResourceData._HealthUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.HealthRegen = (float)ResourceData._HealthRegen.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ManaUp = (PercentUp)ResourceData._ManaUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ManaRegen = (float)ResourceData._ManaRegen.GetValueInRange(ran.ExNextFloatNormalized());
        Option.BaseAttackAdd = (float)ResourceData._BaseAttackAdd.GetValueInRange(ran.ExNextFloatNormalized());
        Option.PhyAttack = (Percent)ResourceData._PhyAttack.GetValueInRange(ran.ExNextFloatNormalized());
        Option.FireAttack = (Percent)ResourceData._FireAttack.GetValueInRange(ran.ExNextFloatNormalized());
        Option.IceAttack = (Percent)ResourceData._IceAttack.GetValueInRange(ran.ExNextFloatNormalized());
        Option.LightningAttack = (Percent)ResourceData._LightningAttack.GetValueInRange(ran.ExNextFloatNormalized());
        Option.DefenceUp = (PercentUp)ResourceData._DefenceUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.MoveSpeedUp = (PercentUp)ResourceData._MoveSpeedUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.AttackSpeedUp = (PercentUp)ResourceData._AttackSpeedUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.CooltimeDown = (PercentUp)ResourceData._CooltimeDown.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ShieldAdd = (float)ResourceData._ShieldAdd.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ShieldRegen = (float)ResourceData._ShieldRegen.GetValueInRange(ran.ExNextFloatNormalized());
        Option.CriticalRate = (PercentUp)ResourceData._CriticalRate.GetValueInRange(ran.ExNextFloatNormalized());
        Option.CriticalAttack = (PercentUp)ResourceData._CriticalAttack.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ProjectileCountUp = (float)ResourceData._ProjectileCountUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ProjectileSpeedUp = (PercentUp)ResourceData._ProjectileSpeedUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.AttackRangeUp = (PercentUp)ResourceData._AttackRangeUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.SplashRangeUp = (PercentUp)ResourceData._SplashRangeUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.DurationUp = (PercentUp)ResourceData._DurationUp.GetValueInRange(ran.ExNextFloatNormalized());
        Option.FireResist = (Percent)ResourceData._FireResist.GetValueInRange(ran.ExNextFloatNormalized());
        Option.IceResist = (Percent)ResourceData._IceResist.GetValueInRange(ran.ExNextFloatNormalized());
        Option.LightningResist = (Percent)ResourceData._LightningResist.GetValueInRange(ran.ExNextFloatNormalized());
        Option.PosionResist = (Percent)ResourceData._PosionResist.GetValueInRange(ran.ExNextFloatNormalized());

        UpdateDisplayInfo();
    }

    public void UpdateDisplayInfo()
    {
        DisplayInfo.Clear();
        if (Option == null)
            return;

        List<FieldData> fields = new List<FieldData>();
        ReflectionFieldExtractor.GetFields(Option, fields);
        foreach (var field in fields)
        {
            if (field.Value.Equals("0") || field.Value.Equals("0%"))
                continue;

            DisplayInfo[field.Name] = field.Value;
        }
    }
}
