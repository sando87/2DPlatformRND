using UnityEngine;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupInven : PopupBase
    {
        [SerializeField] Transform EquipSlotRoot;
        [SerializeField] Transform InvenSlotRoot;

        public ItemInventory ItemInven { get; set; }

        private UIPartsItemSlot[] mInvenSlots = null;
        private UIPartsItemSlot[] mEquipSlots = null;

        void Start()
        {
            InitSlots();

            InitItemInfo();

            UpdateUIParts();
        }

        void InitSlots()
        {
            mInvenSlots = InvenSlotRoot.GetComponentsInChildren<UIPartsItemSlot>();
            foreach (UIPartsItemSlot slot in mInvenSlots)
            {
                slot.SetEmpty();
                slot.EventSelect.AddListener(() => OnSelectSlot(slot, false));
                slot.EventDeselect.AddListener(() => OnDeselectSlot(slot, false));
                slot.EventSubmit.AddListener(() => OnSubmitSlot(slot, false));
            }

            mEquipSlots = EquipSlotRoot.GetComponentsInChildren<UIPartsItemSlot>();
            foreach (UIPartsItemSlot slot in mEquipSlots)
            {
                slot.SetEmpty();
                slot.EventSelect.AddListener(() => OnSelectSlot(slot, true));
                slot.EventDeselect.AddListener(() => OnDeselectSlot(slot, true));
                slot.EventSubmit.AddListener(() => OnSubmitSlot(slot, true));
            }
        }

        void InitItemInfo()
        {
            ItemInfo[] invenItems = ItemInven.ListInvenItems();
            int count = Mathf.Min(invenItems.Length, mInvenSlots.Length);
            for (int i = 0; i < count; ++i)
            {
                UIPartsItemSlot itemSlot = mInvenSlots[i];
                ItemInfo itemInfo = invenItems[i];
                itemSlot.SetItemInfo(itemInfo);
            }

            ItemInfo[] equipItems = ItemInven.ListEquipItems();
            count = Mathf.Min(equipItems.Length, mEquipSlots.Length);
            for (int i = 0; i < count; ++i)
            {
                UIPartsItemSlot itemSlot = mEquipSlots[i];
                ItemInfo itemInfo = equipItems[i];
                itemSlot.SetItemInfo(itemInfo);
            }
        }

        void OnSelectSlot(UIPartsItemSlot slot, bool isEquipSlot)
        {
            slot.GetComponent<Image>().color = Color.green;

            if (slot.IsEmpty) return;

        }
        void OnDeselectSlot(UIPartsItemSlot slot, bool isEquipSlot)
        {
            slot.GetComponent<Image>().color = Color.white;

            if (slot.IsEmpty) return;

        }
        void OnSubmitSlot(UIPartsItemSlot slot, bool isEquipSlot)
        {
            slot.GetComponent<Image>().color = Color.red;
            this.ExDelayedCoroutine(0.2f, () => slot.GetComponent<Image>().color = Color.green);

            if (slot.IsEmpty) return;

            if (isEquipSlot)
            {
                UnEquipItem(slot);
            }
            else
            {
                EquipItem(slot);
            }

        }

        void ShowItemInfo(UIPartsItemSlot slot)
        {

        }
        void HideItemInfo(UIPartsItemSlot slot)
        {

        }
        void ShowSlotActions(UIPartsItemSlot slot)
        {
            // show popup
            // get return
            // switch case 

        }
        void HideSlotActions(UIPartsItemSlot slot)
        {

        }

        void EquipItem(UIPartsItemSlot slot)
        {
            UIPartsItemSlot emptySlot = FindEmptyEquipSlot();
            if (emptySlot != null)
            {
                ItemInfo itemInfo = slot.ItemInfo;
                emptySlot.SetItemInfo(itemInfo);
                slot.SetEmpty();

                // ItemInven.EquipItem(itemInfo.InstanceID);
            }
            else
            {
                // There is no empty slot for equipment.
            }
        }
        void UnEquipItem(UIPartsItemSlot slot)
        {
            UIPartsItemSlot emptySlot = FindEmptyInvenSlot();
            if (emptySlot != null)
            {
                ItemInfo itemInfo = slot.ItemInfo;
                emptySlot.SetItemInfo(itemInfo);
                slot.SetEmpty();

                // ItemInven.UnEquipItem(itemInfo.InstanceID);
            }
            else
            {
                // There is no empty slot for unequipment.
            }
        }

        UIPartsItemSlot FindEmptyEquipSlot()
        {
            for (int i = 0; i < mEquipSlots.Length; ++i)
            {
                if (mEquipSlots[i].IsEmpty)
                    return mEquipSlots[i];
            }
            return null;
        }
        UIPartsItemSlot FindEmptyInvenSlot()
        {
            for (int i = 0; i < mInvenSlots.Length; ++i)
            {
                if (mInvenSlots[i].IsEmpty)
                    return mInvenSlots[i];
            }
            return null;
        }




    }


}