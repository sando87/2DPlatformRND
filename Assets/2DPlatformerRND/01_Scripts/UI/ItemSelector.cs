using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class ItemSelector : MonoBehaviour
    {
        [SerializeField] Transform _SelectorParent = null;
        [SerializeField] UIActionSelector _ActionSelector = null;

        UIActionSelector mItemSelector = null;
        List<ItemObject> mItemObjs = null;
        List<string> mActions = new List<string>();

        public void ShowItemSelector(List<ItemObject> itemObjs)
        {
            if (mItemSelector != null)
            {
                Destroy(mItemSelector.gameObject);
                mItemSelector = null;
            }

            mItemObjs = itemObjs;

            mActions.Clear();
            foreach (ItemObject itemInfo in itemObjs)
            {
                mActions.Add(itemInfo.ItemInfo.Name);
            }

            mItemSelector = UIActionSelector.Show(_ActionSelector, _SelectorParent, mActions, null);
            UIPartsHandler[] buttons = mItemSelector.ActionButtons;
            for (int i = 0; i < buttons.Length; i++)
            {
                UIPartsHandler button = buttons[i];
                if (!button.gameObject.activeInHierarchy)
                    continue;

                ItemObject itemObj = mItemObjs[i];
                button.EventSelect = (btn) => Select(btn, itemObj);
                button.EventDeselect = (btn) => DeSelect(btn, itemObj);
                button.EventSubmit = (btn) => Submit(btn, itemObj);
            }
        }

        public void HideItemSelector()
        {
            if (mItemSelector != null)
            {
                Destroy(mItemSelector.gameObject);
                mItemSelector = null;
            }
        }

        void Select(UIPartsHandler button, ItemObject itemobj)
        {
            button.GetComponent<Image>().color = Color.green;
            ShowItemInfo(itemobj);
        }
        void DeSelect(UIPartsHandler button, ItemObject itemobj)
        {
            button.GetComponent<Image>().color = Color.white;
            HideItemInfo(itemobj);
        }
        void Submit(UIPartsHandler button, ItemObject itemobj)
        {
            PickItemUp(itemobj);
        }
        void ShowItemInfo(ItemObject itemobj)
        {

        }
        void HideItemInfo(ItemObject itemobj)
        {

        }
        void PickItemUp(ItemObject itemobj)
        {
            InGameEngine.Instance.Player.GetComponentInChildren<ItemInventory>().AddItem(itemobj.ItemInfo);
            itemobj.OnPickedUp();
            HideItemSelector();
        }
    }
}