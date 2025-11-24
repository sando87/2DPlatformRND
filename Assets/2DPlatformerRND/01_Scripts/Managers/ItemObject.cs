using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemObject
{
    public ItemSaveData SaveData { get; private set; } = null;
    public ItemResourceData ResourceData { get; private set; } = null;
    public StatsOption Option { get; private set; } = null;

    public string InstanceID => SaveData.InstanceID;
    public long ResourceID => ResourceData.ID;
    public bool IsEquipped { get => SaveData.IsEquipped; set => SaveData.IsEquipped = value; }
    public int Count { get => SaveData.Count; set => SaveData.Count = value; }
    public int PositionIndex { get => SaveData.PositionIndex; set { SaveData.PositionIndex = value; } }
    public int Level { get => SaveData.Level; set { SaveData.Level = value; UpdateOption(); } }

    public void CreateRandomItem()
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
        Option = new StatsOption();
        int point = SaveData.LevelIndex;
        System.Random ran = new System.Random(SaveData.RandomSeed);

        Option.HealthUp = ResourceData.HealthUpPair.GetValue(point);
        Option.HealthRegen = ResourceData.HealthRegen;
        Option.MoveSpeedUp = ResourceData.MoveSpeedUpPercent;
        Option.ShieldAdd = ResourceData.ShieldAddRange.GetDouble(ran.NextDouble());
    }
}
