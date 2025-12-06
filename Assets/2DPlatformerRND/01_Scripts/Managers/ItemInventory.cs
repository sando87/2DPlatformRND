using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ItemInventory : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        private Dictionary<string, ItemInfo> mInvenItems = new Dictionary<string, ItemInfo>();
        private Dictionary<string, ItemInfo> mEquipItems = new Dictionary<string, ItemInfo>();
        private Dictionary<string, ItemSaveData> mSaveData = null; 

        public ItemStats TotalItemOption { get; private set; } = new ItemStats();

        public double CurrentGold { get; set; } = 0;

        [ShowIf(nameof(ShowInvenItems))]
        [Dropdown(nameof(ListInvenItems))]
        [OnValueChanged(nameof(SelectInvenItem))]
        public ItemInfo mSelectedInvenItem = null;
        ItemInfo[] ListInvenItems() { return mInvenItems.Values.ToArray(); }

        [SerializeField]
        [ShowIf(nameof(ShowInvenItems))]
        private ItemInfo _SelectInvenItem = null;
        void SelectInvenItem() { _SelectInvenItem = mSelectedInvenItem; _SelectInvenItem._Option = mSelectedInvenItem.Option; }

        [ShowIf(nameof(ShowEquipItems))]
        [Dropdown(nameof(ListEquipItems))]
        [OnValueChanged(nameof(SelectEquipItem))]
        public ItemInfo mSelectEquipItem = null;
        ItemInfo[] ListEquipItems() { return mEquipItems.Values.ToArray(); }

        [SerializeField]
        [ShowIf(nameof(ShowEquipItems))]
        private ItemInfo _SelectEquipItem = null;
        void SelectEquipItem() { _SelectEquipItem = mSelectEquipItem; _SelectEquipItem._Option = mSelectEquipItem.Option; }


        bool ShowInvenItems() { return Application.isPlaying && mSaveData != null && mInvenItems.Count > 0; }
        bool ShowEquipItems() { return Application.isPlaying && mSaveData != null && mEquipItems.Count > 0; }

        [Button]
        [ShowIf(nameof(ShowInvenItems))]
        void _EquipItem() { EquipItem(mSelectedInvenItem.InstanceID); }

        [Button]
        [ShowIf(nameof(ShowEquipItems))]
        void _UnEquipItem() { UnEquipItem(mSelectEquipItem.InstanceID); }


        void Awake()
        {
            LoadItemsFromData();
        }

        void LoadItemsFromData()
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;
            mSaveData = saveData.Characters[charID].Items;
            foreach (var pair in mSaveData)
            {
                ItemSaveData itemSaveData = pair.Value;
                ItemInfo item = new ItemInfo();
                item.LoadItem(itemSaveData);

                if (item.IsEquipped)
                {
                    mEquipItems[itemSaveData.InstanceID] = item;
                    TotalItemOption.Add(item.Option);
                }
                else
                {
                    mInvenItems[itemSaveData.InstanceID] = item;
                }
            }
        }

        public void AddItem(ItemInfo item)
        {
            if (mInvenItems.ContainsKey(item.InstanceID))
            {
                mInvenItems[item.InstanceID].Count += item.Count;
            }
            else
            {
                mInvenItems[item.InstanceID] = item;
                mSaveData[item.InstanceID] = item.SaveData;
            }
            GameSystem.DoSave_UserSaveData();
        }
        public void RemoveItem(string itemInstID)
        {
            LOG.errorif(!mSaveData.ContainsKey(itemInstID));
            mSaveData.Remove(itemInstID);
            if (mInvenItems.ContainsKey(itemInstID))
                mInvenItems.Remove(itemInstID);
            if (mEquipItems.ContainsKey(itemInstID))
                mEquipItems.Remove(itemInstID);
            GameSystem.DoSave_UserSaveData();
        }
        public ItemInfo GetItem(string itemInstID)
        {
            if (mInvenItems.ContainsKey(itemInstID))
                return mInvenItems[itemInstID];
            else if (mEquipItems.ContainsKey(itemInstID))
                return mEquipItems[itemInstID];
            else
                return null;
        }

        public void SubItem(string itemInstID, int count)
        {
            GetItem(itemInstID).Count -= count;
            GameSystem.DoSave_UserSaveData();
        }
        public void UpgradeItem(string itemInstID)
        {
            GetItem(itemInstID).Level++;
            GameSystem.DoSave_UserSaveData();
        }

        public void MoveItem(string itemInstID, int newPositionIndex)
        {
            GetItem(itemInstID).PositionIndex = newPositionIndex;
            GameSystem.DoSave_UserSaveData();
        }

        public void EquipItem(string itemInstID)
        {
            ItemInfo item = GetItem(itemInstID);
            if(item.IsEquipped)
                return;

            item.IsEquipped = true;
            mInvenItems.Remove(itemInstID);
            mEquipItems.Add(itemInstID, item);
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Add(item.Option);
        }

        public void UnEquipItem(string itemInstID)
        {
            ItemInfo item = GetItem(itemInstID);
            if(!item.IsEquipped)
                return;

            item.IsEquipped = false;
            mEquipItems.Remove(itemInstID);
            mInvenItems.Add(itemInstID, item);
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Subtract(item.Option);
        }


    }
}