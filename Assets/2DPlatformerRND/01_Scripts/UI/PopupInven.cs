using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupInven : PopupBase
    {
        [SerializeField] Transform EquipSlotRoot;
        [SerializeField] Transform InvenSlotRoot;
        [SerializeField] Transform OptionContentRoot;
        [SerializeField] UIPartsFieldRow FieldRow;
        [SerializeField] UIPartsActions _ActionSelector;

        public ItemInventory ItemInven { get; set; }

        private UIPartsItemSlot[] mInvenSlots = null;
        private UIPartsItemSlot[] mEquipSlots = null;
        private List<FieldData> mDisplayFields = new List<FieldData>();

        void Start()
        {
            InitSlots();

            InitItemInfo();

            UpdateDisplayInfo(null);

            _ActionSelector.gameObject.SetActive(false);
        }

        void InitSlots()
        {
            mInvenSlots = InvenSlotRoot.GetComponentsInChildren<UIPartsItemSlot>();
            foreach (UIPartsItemSlot slot in mInvenSlots)
            {
                slot.SetEmpty();
                slot.EventSelect = (btn) => OnSelectSlot(btn, false);
                slot.EventDeselect = (btn) => OnDeselectSlot(btn, false);
                slot.EventSubmit = (btn) => OnSubmitSlot(btn, false);
            }

            mEquipSlots = EquipSlotRoot.GetComponentsInChildren<UIPartsItemSlot>();
            foreach (UIPartsItemSlot slot in mEquipSlots)
            {
                slot.SetEmpty();
                slot.EventSelect = (btn) => OnSelectSlot(btn, true);
                slot.EventDeselect = (btn) => OnDeselectSlot(btn, true);
                slot.EventSubmit = (btn) => OnSubmitSlot(btn, true);
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

        void UpdateDisplayInfo(UIPartsItemSlot selectedSlot)
        {
            OptionContentRoot.ExDestroyAllChildren();

            if (selectedSlot != null)
            {
                if (!selectedSlot.IsEmpty)
                {
                    var displayInfo = selectedSlot.ItemInfo.DisplayInfo;
                    foreach (var kvp in displayInfo)
                    {
                        UIPartsFieldRow row = Instantiate(FieldRow, OptionContentRoot);
                        row.SetField(kvp.Key, kvp.Value);
                    }
                }
            }
            else
            {
                ReflectionFieldExtractor.GetFields(ItemInven.TotalItemOption, mDisplayFields);
                foreach (var field in mDisplayFields)
                {
                    string val = field.Value;
                    if (val.Equals("0") || val.Equals("0%"))
                        continue;

                    UIPartsFieldRow row = Instantiate(FieldRow, OptionContentRoot);
                    row.SetField(field.Name, field.Value);
                }
            }
        }

        void OnSelectSlot(UIPartsHandler part, bool isEquipSlot)
        {
            UIPartsItemSlot slot = part as UIPartsItemSlot;
            slot.GetComponent<Image>().color = Color.green;

            UpdateDisplayInfo(slot);

            if (slot.IsEmpty) return;

        }
        void OnDeselectSlot(UIPartsHandler part, bool isEquipSlot)
        {
            UIPartsItemSlot slot = part as UIPartsItemSlot;
            slot.GetComponent<Image>().color = Color.white;

            if (slot.IsEmpty) return;

        }
        void OnSubmitSlot(UIPartsHandler part, bool isEquipSlot)
        {
            UIPartsItemSlot slot = part as UIPartsItemSlot;
            if (slot.IsEmpty) return;

            ShowActionSelector(slot);
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

                ItemInven.EquipItem(itemInfo.InstanceID);
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

                ItemInven.UnEquipItem(itemInfo.InstanceID);
            }
        }
        void DumpItem(UIPartsItemSlot slot)
        {
            ItemInfo itemInfo = slot.ItemInfo;
            slot.SetEmpty();

            ItemInven.RemoveItem(itemInfo.InstanceID);
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

        void ShowActionSelector(UIPartsHandler part)
        {
            UIPartsItemSlot itemSlot = part as UIPartsItemSlot;

            InGameManager.Instance.Engine.SetInputHandler(_ActionSelector.InputHandler);

            _ActionSelector.Actions.Clear();
            if (itemSlot.ItemInfo.IsEquipped)
            {
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.UnEquip });
            }
            else
            {
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Equip });
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Dump });
            }

            _ActionSelector.Show((type) =>
            {
                DoAction(itemSlot, type);
                InGameManager.Instance.Engine.SetInputHandler(this.InputHandler);
                InputHandler.SelectUIPart(part);
                UpdateDisplayInfo(null);
            });

            SetRePosition(_ActionSelector.transform, part);
        }

        void SetRePosition(Transform target, UIPartsHandler part)
        {
            target.position = part.transform.position;

            RectTransform screenArea = GetComponent<RectTransform>();
            target.GetComponent<RectTransform>().MoveInsideOf(screenArea);
        }

        void DoAction(UIPartsItemSlot itemSlot, UIActionType type)
        {
            if (type == UIActionType.None)
                return;

            if (type == UIActionType.Equip)
            {
                EquipItem(itemSlot);
            }
            else if (type == UIActionType.UnEquip)
            {
                UnEquipItem(itemSlot);
            }
            else if (type == UIActionType.Dump)
            {
                DumpItem(itemSlot);
            }
        }



    }


}