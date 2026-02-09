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

        Option.HealthUp = (Percent)ResourceData._HealthUp.GetValueByPoint(point);
        Option.HealthRegen = ResourceData._HealthRegen.GetValue();
        Option.ManaUp = (Percent)ResourceData._ManaUp.GetValueByPoint(point);
        Option.ManaRegen = ResourceData._ManaRegen.GetValue();
        Option.BaseAttackAdd = ResourceData._BaseAttackAdd.GetValueByPoint(point);
        Option.PhyAttackUp = (Percent)ResourceData._AttackUp.GetValueByPoint(point);
        Option.FireAttackUp = (Percent)ResourceData._FireAttackUp.GetValueByPoint(point);
        Option.IceAttackUp = (Percent)ResourceData._IceAttackUp.GetValueByPoint(point);
        Option.LightningAttackUp = (Percent)ResourceData._LightningAttackUp.GetValueByPoint(point);
        Option.DefenceUp = (Percent)ResourceData._DefenceUp.GetValueByPoint(point);
        Option.MoveSpeedUp = (Percent)ResourceData._MoveSpeedUp.GetValue();
        Option.AttackSpeedUp = (Percent)ResourceData._AttackSpeedUp.GetValue();
        Option.CooltimeDown = (Percent)ResourceData._CooltimeDown.GetValue();
        Option.ShieldAdd = ResourceData._ShieldAdd.GetValueInRange(ran.ExNextFloatNormalized());
        Option.ShieldRegen = ResourceData._ShieldRegen.GetValue();
        Option.CriticalRate = (Percent)ResourceData._CriticalRate.GetValue();
        Option.CriticalAttack = (Percent)ResourceData._CriticalAttack.GetValue();
        Option.ProjectileCountUp = ResourceData._ProjectileCountUp.GetValue();
        Option.ProjectileSpeedUp = (Percent)ResourceData._ProjectileSpeedUp.GetValue();
        Option.AttackRangeUp = (Percent)ResourceData._AttackRangeUp.GetValue();
        Option.SplashRangeUp = (Percent)ResourceData._SplashRangeUp.GetValue();
        Option.DurationUp = (Percent)ResourceData._DurationUp.GetValue();
        Option.FireResist = (Percent)ResourceData._FireResist.GetValue();
        Option.IceResist = (Percent)ResourceData._IceResist.GetValue();
        Option.LightningResist = (Percent)ResourceData._LightningResist.GetValue();
        Option.PosionResist = (Percent)ResourceData._PosionResist.GetValue();
    }
}
