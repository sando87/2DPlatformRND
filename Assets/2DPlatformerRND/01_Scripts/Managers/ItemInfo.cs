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

        Option.HealthUp = ResourceData._HealthUp.GetValueByPoint(point);
        Option.HealthRegen = ResourceData._HealthRegen.GetValue();
        Option.ManaUp = ResourceData._ManaUp.GetValueByPoint(point);
        Option.ManaRegen = ResourceData._ManaRegen.GetValue();
        Option.AttackUp = ResourceData._AttackUp.GetValueByPoint(point);
        Option.DefenceUp = ResourceData._DefenceUp.GetValueByPoint(point);
        Option.MoveSpeedUp = ResourceData._MoveSpeedUp.GetValue();
        Option.AttackSpeedUp = ResourceData._AttackSpeedUp.GetValue();
        Option.CooltimeDown = ResourceData._CooltimeDown.GetValue();
        Option.ShieldAdd = ResourceData._ShieldAdd.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ShieldRegen = ResourceData._ShieldRegen.GetValue();
        Option.CriticalRate = ResourceData._CriticalRate.GetValue();
        Option.CriticalAttack = ResourceData._CriticalAttack.GetValue();
        Option.ProjectileCountUp = ResourceData._ProjectileCountUp.GetValue();
        Option.ProjectileSpeedUp = ResourceData._ProjectileSpeedUp.GetValue();
        Option.AttackRangeUp = ResourceData._AttackRangeUp.GetValue();
        Option.SplashRangeUp = ResourceData._SplashRangeUp.GetValue();
        Option.DurationUp = ResourceData._DurationUp.GetValue();
        Option.FireResist = ResourceData._FireResist.GetValue();
        Option.IceResist = ResourceData._IceResist.GetValue();
        Option.LightningResist = ResourceData._LightningResist.GetValue();
        Option.PosionResist = ResourceData._PosionResist.GetValue();
    }
}
