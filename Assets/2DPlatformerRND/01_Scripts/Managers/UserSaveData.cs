using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    [System.Serializable]
    public class UserSaveData : SaveableBase
    {
        public int Level = 1;
        public float Exp = 0;
        public float Gold = 0;

        public Dictionary<string, ItemSaveInfo> Items = new Dictionary<string, ItemSaveInfo>();
    }

    [System.Serializable]
    public class ItemSaveInfo
    {
        public string ItemID;
        public bool IsEquipped;
        public int Level;
        public int Count;
        public int RandomSeed;

        public int LevelIndex { get => Level - 1; }
    }
}