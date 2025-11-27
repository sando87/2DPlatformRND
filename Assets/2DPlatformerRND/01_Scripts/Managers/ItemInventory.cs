using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ItemInventory : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        private Dictionary<string, ItemObject> mItems = new Dictionary<string, ItemObject>();

        public ItemStats TotalItemOption { get; private set; } = new ItemStats();

        public double CurrentGold { get; set; } = 0;

        public void Init()
        {
            LoadItemsFromData();
        }

        void LoadItemsFromData()
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;
            var savedItems = saveData.Characters[charID].Items;
            foreach (var pair in savedItems)
            {
                ItemSaveData itemData = pair.Value;
                ItemObject item = new ItemObject();
                item.LoadItem(itemData);

                mItems[itemData.InstanceID] = item;

                if (item.IsEquipped)
                {
                    TotalItemOption.Add(item.Option);
                }
            }
        }

        public void AddItem(ItemObject item)
        {
            if (mItems.ContainsKey(item.InstanceID))
            {
                mItems[item.InstanceID].Count += item.Count;
            }
            else
            {
                mItems[item.InstanceID] = item;
                UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
                int charID = CharRoot.CharacterID;
                saveData.Characters[charID].Items[item.InstanceID] = item.SaveData;
            }
            GameSystem.DoSave_UserSaveData();
        }
        public void RemoveItem(string itemInstID)
        {
            LOG.errorif(!mItems.ContainsKey(itemInstID));
            if (mItems.ContainsKey(itemInstID))
            {
                mItems.Remove(itemInstID);

                UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
                int charID = CharRoot.CharacterID;
                saveData.Characters[charID].Items.Remove(itemInstID);
                GameSystem.DoSave_UserSaveData();
            }
        }

        public void SubItem(string itemInstID, int count)
        {
            LOG.errorif(!mItems.ContainsKey(itemInstID));
            mItems[itemInstID].Count -= count;
            GameSystem.DoSave_UserSaveData();
        }
        public void UpgradeItem(string itemInstID)
        {
            LOG.errorif(!mItems.ContainsKey(itemInstID));
            mItems[itemInstID].Level++;
            GameSystem.DoSave_UserSaveData();
        }

        public void MoveItem(string itemInstID, int newPositionIndex)
        {
            LOG.errorif(!mItems.ContainsKey(itemInstID));
            mItems[itemInstID].PositionIndex = newPositionIndex;
            GameSystem.DoSave_UserSaveData();
        }

        public void EquipItem(string itemInstID)
        {
            LOG.errorif(!mItems.ContainsKey(itemInstID));
            if (mItems[itemInstID].IsEquipped)
                return;

            mItems[itemInstID].IsEquipped = true;
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Add(mItems[itemInstID].Option);
        }

        public void UnEquipItem(string itemInstID)
        {
            LOG.errorif(!mItems.ContainsKey(itemInstID));
            if (!mItems[itemInstID].IsEquipped)
                return;

            mItems[itemInstID].IsEquipped = false;
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Subtract(mItems[itemInstID].Option);
        }


    }
}