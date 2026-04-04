using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PahlBit
{
    public class PopupSkill : PopupBase
    {
        [SerializeField] UIPartsActions _ActionSelector;
        [SerializeField] UIPartsViewer _Viewer;

        [SerializeField] Transform ContentsRoot;
        [SerializeField] UIPartsFieldRow FieldRow;

        private List<FieldData> mDisplayFields = new List<FieldData>();

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
            // ShowViewer(part);
            UpdateDisplayInfo(part);
        }
        void OnDeselect(UIPartsHandler part)
        {
            part.GetComponent<Image>().color = Color.white;
            // HideViewer();
        }
        void OnSubmit(UIPartsHandler part)
        {
            ShowActionSelector(part);
        }

        void ShowActionSelector(UIPartsHandler part)
        {
            UIPartsSkillSlot skillSlot = part as UIPartsSkillSlot;
            SkillBase skill = skillSlot.SKill;

            InGameEngine.Instance.SetInputHandler(_ActionSelector.InputHandler);

            _ActionSelector.Actions.Clear();
            if (!skill.IsLearned)
            {
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Learn });
            }
            else
            {
                _ActionSelector.Actions.Add(new ActionInfo { Type = skill.IsEquipped ? UIActionType.UnEquip : UIActionType.Equip });
                _ActionSelector.Actions.Add(new ActionInfo { Type = UIActionType.Enforce });
            }

            _ActionSelector.Show((type) =>
            {
                DoAction(skill, type);
                skillSlot.UpdateSkillState();
                InGameEngine.Instance.SetInputHandler(this.InputHandler);
                InputHandler.SelectUIPart(part);
            });

            SetRePosition(_ActionSelector.transform, part);
        }

        void DoAction(SkillBase skill, UIActionType type)
        {
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
            SkillStats basicStats = skillPart.SKill.Spec.BaseStats;
            ReflectionFieldExtractor.GetFields(basicStats, _Viewer.Data);

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

            ReflectionFieldExtractor.GetFields(skill.Spec.BaseStats, mDisplayFields);
            foreach (var field in mDisplayFields)
            {
                string val = field.Value;
                if (val.Equals("0") || val.Equals("0%"))
                    continue;

                UIPartsFieldRow row = Instantiate(FieldRow, ContentsRoot);
                row.SetField(field.Name, field.Value);
            }
        }
    }
}