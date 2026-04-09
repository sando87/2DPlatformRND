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
        [SerializeField] UIListViewer _ListViewer = null;

        UIActionSelector mItemSelector = null;
        UIListViewer mItemViewer = null;
        List<ItemObject> mItemObjs = null;
        List<UIPartsHandler> mItemButtons = new List<UIPartsHandler>();
        List<string> mActions = new List<string>();
        int mCurrentSelectedIndex = -1;

        public void ShowItemSelector(List<ItemObject> itemObjs)
        {
            if (mItemSelector != null)
            {
                Destroy(mItemSelector.gameObject);
                mItemSelector = null;
            }

            mCurrentSelectedIndex = -1;
            mItemButtons.Clear();
            mActions.Clear();
            mItemObjs = itemObjs;

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
                mItemButtons.Add(button);
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

        public void MoveUp()
        {
            if (mItemSelector == null)
                return;

            mCurrentSelectedIndex--;
            if (mCurrentSelectedIndex < 0)
                mCurrentSelectedIndex = mItemButtons.Count - 1;

            EventSystem.current.SetSelectedGameObject(mItemButtons[mCurrentSelectedIndex].gameObject);
        }
        public void MoveDown()
        {
            if (mItemSelector == null)
                return;

            mCurrentSelectedIndex++;
            if (mCurrentSelectedIndex >= mItemButtons.Count)
                mCurrentSelectedIndex = 0;

            EventSystem.current.SetSelectedGameObject(mItemButtons[mCurrentSelectedIndex].gameObject);
        }

        void Select(UIPartsHandler button, ItemObject itemobj)
        {
            button.GetComponent<Image>().color = Color.green;
            ShowItemInfo(button, itemobj);
        }
        void DeSelect(UIPartsHandler button, ItemObject itemobj)
        {
            button.GetComponent<Image>().color = Color.white;
            HideItemInfo(itemobj);
        }
        void Submit(UIPartsHandler button, ItemObject itemobj)
        {
            PickItemUp(itemobj);
            RemoveCurrentButton(button);
        }
        void RemoveCurrentButton(UIPartsHandler button)
        {
            if (mCurrentSelectedIndex == mItemButtons.Count - 1)
            {
                mCurrentSelectedIndex--;
            }
            mItemButtons.Remove(button);
            Destroy(button.gameObject);

            if (mItemButtons.Count > 0 && mCurrentSelectedIndex < mItemButtons.Count)
                EventSystem.current.SetSelectedGameObject(mItemButtons[mCurrentSelectedIndex].gameObject);
        }
        void ShowItemInfo(UIPartsHandler button, ItemObject itemobj)
        {
            List<string> infos = new List<string>();
            foreach (var field in itemobj.ItemInfo.DisplayInfo)
            {
                infos.Add(field.Key + "," + field.Value);
            }
            mItemViewer = UIListViewer.Show(_ListViewer, button.transform, infos);
        }
        void HideItemInfo(ItemObject itemobj)
        {
            if (mItemViewer != null)
            {
                Destroy(mItemViewer.gameObject);
                mItemViewer = null;
            }
        }
        void PickItemUp(ItemObject itemobj)
        {
            InGameManager.Instance.Engine.Player.GetComponentInChildren<ItemInventory>().AddItem(itemobj.ItemInfo);
            itemobj.OnPickedUp();
        }
    }
}