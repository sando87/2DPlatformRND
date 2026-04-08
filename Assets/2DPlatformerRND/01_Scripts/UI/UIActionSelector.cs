using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIActionSelector : MonoBehaviour
    {
        [SerializeField] UIPartsHandler[] _ActionButtons;
        [SerializeField] UIInputHandler _InputHandler;

        public UIInputHandler InputHandler { get => _InputHandler; }
        public UIPartsHandler[] ActionButtons { get => _ActionButtons; }

        private IInputHandler mReturnInputHandler;
        private List<string> mActions;

        public static UIActionSelector Show(UIActionSelector prefab, Transform parent, List<string> actions, Action<string> onEnd)
        {
            UIActionSelector actionSelector = Instantiate(prefab, parent);

            actionSelector.mReturnInputHandler = InGameEngine.Instance.GetInputHandler();
            InGameEngine.Instance.SetInputHandler(actionSelector.InputHandler);

            actionSelector.mActions = actions;

            actionSelector.Show((type) =>
            {
                onEnd?.Invoke(type);
            });
            return actionSelector;
        }

        void OnDestroy()
        {
            if (mReturnInputHandler != null)
                InGameEngine.Instance.SetInputHandler(mReturnInputHandler);
        }

        public void Show(Action<string> onEnd)
        {
            int count = _ActionButtons.Length;
            for (int i = 0; i < count; ++i)
            {
                UIPartsHandler actionBtn = _ActionButtons[i];
                if (i < mActions.Count)
                {
                    string actionName = mActions[i];
                    actionBtn.gameObject.SetActive(true);
                    SetButton(actionBtn, actionName);
                    actionBtn.EventSelect = Select;
                    actionBtn.EventDeselect = DeSelect;
                    actionBtn.EventSubmit = (btn) =>
                    {
                        onEnd(actionName);
                        Destroy(gameObject);
                    };
                }
                else
                {
                    actionBtn.gameObject.SetActive(false);
                }
            }

            _InputHandler.EventCancel = () =>
            {
                onEnd("");
                Destroy(gameObject);
            };

            _InputHandler.SelectUIPart(_ActionButtons[0]);

            RectTransform screenArea = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            GetComponent<RectTransform>().MoveInsideOf(screenArea);
        }

        void SetButton(UIPartsHandler button, string data)
        {
            button.GetComponent<Image>().color = Color.white;

            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = data;
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