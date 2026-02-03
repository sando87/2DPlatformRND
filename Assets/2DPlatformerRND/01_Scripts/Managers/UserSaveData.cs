using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    [System.Serializable]
    public class UserSaveData : SaveableBase
    {
        public int Gold = 0;

        public Dictionary<int, CharacterSaveData> Characters = new Dictionary<int, CharacterSaveData>();
    }

    [System.Serializable]
    public class CharacterSaveData
    {
        public CharSaveData Stats = new CharSaveData();
        public Dictionary<string, ItemSaveData> Items = new Dictionary<string, ItemSaveData>();
        public Dictionary<long, SkillSaveData> Skills = new Dictionary<long, SkillSaveData>();
    }

    [System.Serializable]
    public class CharSaveData
    {
        public float CurrentExp;
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

    [System.Serializable]
    public class SkillSaveData
    {
        public long ResourceID;
        public bool IsEquipped;
        public bool IsLearned;
        public int Level;
        public int PositionIndex;

        public int LevelIndex { get => Level - 1; }

        public SkillSaveData(long _resourceID)
        {
            this.ResourceID = _resourceID;
        }
    }
}