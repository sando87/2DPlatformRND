using System;
using UnityEngine;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupSkill : PopupBase
    {
        [SerializeField] UIPartsActions _ActionSelector;
        [SerializeField] UIPartsViewer _Viewer;

        void Start()
        {
            UIPartsHandler[] parts = InputHandler.UIParts;
            foreach (var part in parts)
            {
                part.EventSelect = OnSelect;
                part.EventDeselect = OnDeselect;
                part.EventSubmit = OnSubmit;
            }

            _ActionSelector.gameObject.SetActive(false);
        }

        void OnSelect(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.green;
            ShowViewer(part);
        }
        void OnDeselect(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.white;
            HideViewer();
        }
        void OnSubmit(UIPartsHandler part)
        {
            InGameEngine.Instance.SetInputHandler(_ActionSelector.InputHandler);

            _ActionSelector.Actions.Clear();
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Learn, isEnabled = true });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Use, isEnabled = true });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Enforce, isEnabled = false });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Equip, isEnabled = true });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.UnEquip, isEnabled = true });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.UnUse, isEnabled = false });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Sell, isEnabled = false });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Buy, isEnabled = true });
            _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Dump, isEnabled = false });
            
            _ActionSelector.Show((type) =>
            {
                LOG.trace(type);
                InGameEngine.Instance.SetInputHandler(this.InputHandler);
                InputHandler.SelectUIPart(part);
            });
            
            SetRePosition(_ActionSelector.transform, part);
        }

        void SetRePosition(Transform target, UIPartsHandler part)
        {
            target.position = part.transform.position;

            RectTransform screenArea = GetComponent<RectTransform>();
            target.GetComponent<RectTransform>().MoveInsideOf(screenArea);
        }

        void ShowViewer(UIPartsHandler part)
        {
            _Viewer.Data.Clear();
            _Viewer.Data.Add(new FieldData { Name = "Spec1", Value = "123" });
            _Viewer.Data.Add(new FieldData { Name = "Spec2", Value = "456" });
            _Viewer.Data.Add(new FieldData { Name = "Spec3", Value = "777" });
            _Viewer.Data.Add(new FieldData { Name = "Spec4", Value = "888" });
            _Viewer.Show();
            
            SetRePosition(_Viewer.transform, part);
        }
        void HideViewer()
        {
            _Viewer.Hide();
        }
    }
}