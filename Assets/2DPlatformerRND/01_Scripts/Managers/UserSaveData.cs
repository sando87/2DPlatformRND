using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    [System.Serializable]
    public class UserSaveData : SaveableBase
    {
        public double Gold = 0;

        public Dictionary<int, CharacterSaveData> Characters = new Dictionary<int, CharacterSaveData>();
    }

    [System.Serializable]
    public class CharacterSaveData
    {
        public StatsSaveData Stats = new StatsSaveData();
        public Dictionary<string, ItemSaveData> Items = new Dictionary<string, ItemSaveData>();
    }

    [System.Serializable]
    public class StatsSaveData
    {
        public double CurrentExp;
        public int HealthPoint;
        public int ManaPoint;
        public int AttackPoint;
        public int DefensePoint;
    }

    [System.Serializable]
    public class ItemSaveData
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