using System;
using System.Collections;
using UnityEngine;

namespace PahlBit
{
    public class ResourceManager : SingletonMono<ResourceManager>
    {
        [SerializeField] Gold GoldPrefab;
        [SerializeField] GameObject LifePotionPrefab;
        [SerializeField] GameObject ManaPotionPrefab;

        protected override void Awake()
        {
            base.Awake();
        }

        public Gold GetPrefabGold() { return GoldPrefab; }
        public GameObject GetPrefabLifePotion() { return LifePotionPrefab; }
        public GameObject GetPrefabManaPotion() { return ManaPotionPrefab; }
    }
}
