using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIPartsItemSlot : UIPartsHandler
    {
        [SerializeField] bool _IsEquipSlot = false;

        public bool IsEquipSlot { get => _IsEquipSlot; }
        public ItemInfo ItemInfo { get; private set; }
        public bool IsEmpty { get => ItemInfo == null; }

        private Image mImage = null;

        void Awake()
        {
            mImage = transform.GetChild(0).GetComponent<Image>();
            SetEmpty();
        }

        public void SetItemInfo(ItemInfo itemInfo)
        {
            ItemInfo = itemInfo;
            mImage.sprite = itemInfo.ResourceData.AssetData.Icon;
        }
        public void SetEmpty()
        {
            ItemInfo = null;
            mImage.sprite = null;
        }
    }
}