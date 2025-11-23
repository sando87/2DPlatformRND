using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    [System.Serializable]
    public class UserSaveData : SaveableBase
    {
        public double Gold = 0;

        public PlayerSaveData PlayerData = new PlayerSaveData();
    }

    [System.Serializable]
    public class PlayerSaveData
    {
        public StatsData Stats = new StatsData();
        public List<ItemData> Items = new List<ItemData>();
    }

    [System.Serializable]
    public class StatsData
    {
        public double CurrentExp;
        public int HealthPoint;
        public int ManaPoint;
        public int AttackPoint;
        public int DefensePoint;
    }

    [System.Serializable]
    public class ItemData
    {
        public string InstanceID;
        public long ResourceID;
        public bool IsEquipped;
        public int Level;
        public int Count;
        public int PositionIndex;

        public int RandomSeed { get => InstanceID.GetHashCode(); }
        public int LevelIndex { get => Level - 1; }
    }
}