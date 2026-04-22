using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public enum UIActionType { None, Learn, Use, UnUse, Enforce, Equip, UnEquip, Sell, Buy, Dump, Detail }

    public struct ActionInfo
    {
        public UIActionType Type;
        public bool isEnabled;
        public int Gold;
    }

    public class UIPartsActions : MonoBehaviour
    {
        const string GoldSpriteTextAsset = "<sprite=0>";

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
            if (data.Gold > 0)
            {
                text.text = $"{data.Type}({GoldSpriteTextAsset}{data.Gold})";
            }
            else
            {
                text.text = data.Type.ToString();
            }
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