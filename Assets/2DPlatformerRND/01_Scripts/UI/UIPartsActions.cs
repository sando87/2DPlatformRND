using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public enum UIActionType { None, Learn, Use, UnUse, Enforce, Equip, UnEquip, Sell, Buy, Dump }

    public struct ActionInfo
    {
        public UIActionType Type;
        public bool isEnabled;
    }

    public class UIPartsActions : MonoBehaviour
    {
        public List<ActionInfo> Actions = new List<ActionInfo>();

        [SerializeField] UIPartsHandler[] _ActionButtons;
        [SerializeField] UIInputHandler _InputHandler;

        public UIInputHandler InputHandler { get => _InputHandler; }

        public void Show(Action<UIActionType> onEnd)
        {
            gameObject.SetActive(true);

            int count = _ActionButtons.Length;
            for (int i = 0; i < count; ++i)
            {
                UIPartsHandler actionBtn = _ActionButtons[i];
                if (i < Actions.Count)
                {
                    ActionInfo btnData = Actions[i];
                    actionBtn.gameObject.SetActive(true);
                    SetButton(actionBtn, btnData);
                    actionBtn.EventSelect = Select;
                    actionBtn.EventDeselect = DeSelect;
                    actionBtn.EventSubmit = (btn) =>
                    {
                        onEnd(btnData.Type);
                        Actions.Clear();
                        gameObject.SetActive(false);
                    };
                }
                else
                {
                    actionBtn.gameObject.SetActive(false);
                }
            }

            _InputHandler.EventCancel = () =>
            {
                onEnd(UIActionType.None);
                Actions.Clear();
                gameObject.SetActive(false);
            };

            _InputHandler.SelectUIPart(_ActionButtons[0]);
        }

        void SetButton(UIPartsHandler button, ActionInfo data)
        {
            button.GetComponent<Image>().color = Color.white;

            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = data.Type.ToString();
            // text.color = data.isEnabled ? Color.white : Color.gray;
        }
        void Select(UIPartsHandler button)
        {
            button.GetComponent<Image>().color = Color.green;
        }
        void DeSelect(UIPartsHandler button)
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }
}