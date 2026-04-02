using System;
using UnityEngine;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupSkill : PopupBase
    {
        [SerializeField] UIPartsActions _ActionSelector;

        void Start()
        {
            UIPartsHandler[] parts = InputHandler.UIParts;
            foreach (var part in parts)
            {
                part.EventSelect = OnSelect;
                part.EventDeselect = OnDeselect;
                part.EventSubmit = OnSubmit;
            }
        }

        void OnSelect(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.green;
        }
        void OnDeselect(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.white;
        }
        void OnSubmit(UIPartsHandler part)
        {
            InGameEngine.Instance.SetInputHandler(_ActionSelector.InputHandler);

            _ActionSelector.Actions.Clear();
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Learn, isEnabled = true });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Use, isEnabled = true });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Enforce, isEnabled = false });
            _ActionSelector.Show((type) =>
            {
                LOG.trace(type);
                InGameEngine.Instance.SetInputHandler(this.InputHandler);
            });
        }
    }
}