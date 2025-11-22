using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    [System.Serializable]
    public class UserSaveData : SaveableBase
    {
        public double Gold = 0;

        public PlayerSaveInfo PlayerData = new PlayerSaveInfo();
    }

    [System.Serializable]
    public class PlayerSaveInfo
    {
        public StatsInfo Stats = new StatsInfo();
        public List<ItemInfo> Items = new List<ItemInfo>();
    }

    [System.Serializable]
    public class StatsInfo
    {
        public double CurrentExp;
        public int HealthPoint;
        public int ManaPoint;
        public int AttackPoint;
        public int DefensePoint;
    }

    [System.Serializable]
    public class ItemInfo
    {
        public string InstanceID;
        public long ResourceID;
        public bool IsEquipped;
        public int Level;
        public int Count;
        public int PositionIndex;

        public int RandomSeed { get => InstanceID.GetHashCode(); }
        public int LevelIndex { get => Level - 1; }
        public GameDataItem ResourceData { get => TableItem.Instance.GetInfo(ResourceID); }
    }
}