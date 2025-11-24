using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class Inventory : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        public void Init()
        {
        }

        public void UpdateState()
        {
        }

        public void AddItem(ItemData item)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;

            LOG.errorif(!saveData.Characters.ContainsKey(charID));

            if (saveData.Characters[charID].Items.ContainsKey(item.InstanceID))
            {
                saveData.Characters[charID].Items[item.InstanceID].Count += item.Count;
            }
            else
            {
                saveData.Characters[charID].Items[item.InstanceID] = item;
            }
            SaveFileManager<UserSaveData>.Save(saveData);
        }
        public void SubItem(string itemInstID, int count)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;

            LOG.errorif(!saveData.Characters.ContainsKey(charID) || !saveData.Characters[charID].Items.ContainsKey(itemInstID));

            saveData.Characters[charID].Items[itemInstID].Count -= count;
            if (saveData.Characters[charID].Items[itemInstID].Count <= 0)
            {
                RemoveItem(itemInstID);
            }
            else
            {
                SaveFileManager<UserSaveData>.Save(saveData);
            }
        }
        public void RemoveItem(string itemInstID)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;

            LOG.errorif(!saveData.Characters.ContainsKey(charID) || !saveData.Characters[charID].Items.ContainsKey(itemInstID));

            saveData.Characters[charID].Items.Remove(itemInstID);
            SaveFileManager<UserSaveData>.Save(saveData);
        }

        public void UpgradeItem(string itemInstID)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;

            LOG.errorif(!saveData.Characters.ContainsKey(charID) || !saveData.Characters[charID].Items.ContainsKey(itemInstID));

            saveData.Characters[charID].Items[itemInstID].Level++;
            SaveFileManager<UserSaveData>.Save(saveData);
        }

        public void EquipItem(string itemInstID)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;

            LOG.errorif(!saveData.Characters.ContainsKey(charID) || !saveData.Characters[charID].Items.ContainsKey(itemInstID));

            saveData.Characters[charID].Items[itemInstID].IsEquipped = true;
            SaveFileManager<UserSaveData>.Save(saveData);
        }

        public void UnEquipItem(string itemInstID)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;

            LOG.errorif(!saveData.Characters.ContainsKey(charID) || !saveData.Characters[charID].Items.ContainsKey(itemInstID));

            saveData.Characters[charID].Items[itemInstID].IsEquipped = false;
            SaveFileManager<UserSaveData>.Save(saveData);
        }

        public void MoveItem(string itemInstID, int newPositionIndex)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;

            LOG.errorif(!saveData.Characters.ContainsKey(charID) || !saveData.Characters[charID].Items.ContainsKey(itemInstID));

            saveData.Characters[charID].Items[itemInstID].PositionIndex = newPositionIndex;
            SaveFileManager<UserSaveData>.Save(saveData);
        }


    }
}