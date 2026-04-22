using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupSkill : PopupBase
    {
        [SerializeField] TextMeshProUGUI _RemainSkillPoint;
        [SerializeField] UIPartsActions _ActionSelector;
        [SerializeField] UIPartsViewer _Viewer;

        [SerializeField] Transform ContentsRoot;
        [SerializeField] UIPartsFieldRow FieldRow;

        private List<FieldData> mDisplayFields = new List<FieldData>();
        private SkillController mSkillCtrl = null;

        void Start()
        {
            mSkillCtrl = InGameEngine.Inst.Player.GetComponentInChildren<SkillController>();

            UIPartsHandler[] parts = InputHandler.UIParts;
            foreach (var part in parts)
            {
                part.EventSelect = OnSelect;
                part.EventDeselect = OnDeselect;
                part.EventSubmit = OnSubmit;
            }

            _ActionSelector.gameObject.SetActive(false);

            UpdateRemainSkillPoint();
        }

        void OnSelect(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.green;
            UpdateDisplayInfo(part);
        }
        void OnDeselect(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.white;
            HideViewer();
        }
        void OnSubmit(UIPartsHandler part)
        {
            ShowActionSelector(part);
        }

        void ShowActionSelector(UIPartsHandler part)
        {
            UIPartsSkillSlot skillSlot = part as UIPartsSkillSlot;
            SkillBase skill = skillSlot.SKill;

            InGameManager.Instance.Engine.SetInputHandler(_ActionSelector.InputHandler);

            _ActionSelector.Actions.Clear();
            if (skill.IsLocked)
            {
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Detail });
            }
            else if (!skill.IsLearned)
            {
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Detail });

                if (mSkillCtrl.RemainSkillPoint > 0)
                    _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Learn });
            }
            else
            {
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Detail });
                _ActionSelector.Actions.Add(new ActionInfo { Type = skill.IsEquipped ? UIActionType.UnEquip : UIActionType.Equip });

                if (mSkillCtrl.RemainSkillPoint > 0)
                    _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Enforce, Gold = 123 });
            }

            _ActionSelector.Show((type) =>
            {
                DoAction(part, type);
                UpdateRemainSkillPoint();
                skillSlot.UpdateSkillState();
                InGameManager.Instance.Engine.SetInputHandler(this.InputHandler);
                InputHandler.SelectUIPart(part);
            });

            SetRePosition(_ActionSelector.transform, part);
        }

        void DoAction(UIPartsHandler part, UIActionType type)
        {
            UIPartsSkillSlot skillPart = part as UIPartsSkillSlot;
            SkillBase skill = skillPart.SKill;

            if (type == UIActionType.None)
                return;

            if (type == UIActionType.Learn)
            {
                skill.Controller.LearnNewSkill(skill.ResourceID);
            }
            else if (type == UIActionType.Equip)
            {
                int slotIdx = skill.Controller.FindEmptySkillSlotIndex();
                if (slotIdx >= 0)
                {
                    skill.Controller.EquipSkill(skill.ResourceID, slotIdx);
                }
            }
            else if (type == UIActionType.UnEquip)
            {
                skill.Controller.UnEquipSkill(skill.ResourceID, skill.PositionIndex);
            }
            else if (type == UIActionType.Enforce)
            {
                skill.Controller.LevelupSkill(skill.ResourceID);
            }
            else if (type == UIActionType.Detail)
            {
                ShowViewer(part);
            }
        }

        void SetRePosition(Transform target, UIPartsHandler part)
        {
            target.position = part.transform.position;

            RectTransform screenArea = GetComponent<RectTransform>();
            target.GetComponent<RectTransform>().MoveInsideOf(screenArea);
        }

        void ShowViewer(UIPartsHandler part)
        {
            UIPartsSkillSlot skillPart = part as UIPartsSkillSlot;
            skillPart.SKill.Spec.GetBasicStatInfo(_Viewer.Data);

            _Viewer.Show();

            SetRePosition(_Viewer.transform, part);
        }
        void HideViewer()
        {
            _Viewer.Hide();
        }
        void UpdateDisplayInfo(UIPartsHandler part)
        {
            UIPartsSkillSlot skillSlot = part as UIPartsSkillSlot;
            SkillBase skill = skillSlot.SKill;
            ContentsRoot.ExDestroyAllChildren();

            skill.Spec.GetDisplayInfo(mDisplayFields);
            foreach (var field in mDisplayFields)
            {
                UIPartsFieldRow row = Instantiate(FieldRow, ContentsRoot);
                row.SetField(field.Name, field.Value);
            }
        }

        void UpdateRemainSkillPoint()
        {
            _RemainSkillPoint.text = $"SkillPoint : {mSkillCtrl.RemainSkillPoint}";
        }
    }
}