using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField] Dictionary<string, SkillBase> mAllSkills = new Dictionary<string, SkillBase>();
        [SerializeField] SkillBase[] SkillSlots = null;

        [Foldout("Events")]
        public UnityEvent<SkillBase> OnEquipSkill = new UnityEvent<SkillBase>();
        [Foldout("Events")]
        public UnityEvent<SkillBase> OnUnEquipSkill = new UnityEvent<SkillBase>();

        BaseObject mBaseObj = null;

        void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
        }

        public void InitSkills(int characterID)
        {
            SkillBase[] allSkills = GetComponentsInChildren<SkillBase>();
            foreach (SkillBase skillObj in allSkills)
            {
                skillObj.InitSkillInfo(characterID);
                mAllSkills[skillObj.ResourceID] = skillObj;

                if (!skillObj.IsLearned)
                {
                    skillObj.gameObject.SetActive(false);
                    continue;
                }

                if (skillObj.IsEquipped)
                {
                    SkillSlots[skillObj.PositionIndex] = skillObj;
                }
            }
        }

        public void Update()
        {
            foreach (SkillBase skillObject in SkillSlots)
            {
                if (skillObject != null)
                    skillObject.UpdateSkill();
            }
        }


        public SkillBase GetSkill(string skillID)
        {
            return mAllSkills.GetValueOrDefault(skillID);
        }
        public SkillBase GetEquipSkill(int slotIndex)
        {
            return SkillSlots[slotIndex];
        }
        public void LearnNewSkill(string skillResID)
        {
            SkillBase skill = mAllSkills[skillResID];
            skill.gameObject.SetActive(true);
            skill.OnLearnedSkill();
        }
        public void LevelupSkill(string skillResID)
        {
            SkillBase skill = mAllSkills[skillResID];
            skill.OnLevelupSkill();
        }

        public void EquipSkill(string skillResID, int slotIndex)
        {
            SkillBase skill = mAllSkills[skillResID];
            SkillSlots[slotIndex] = skill;
            SkillSlots[slotIndex].OnEquipedSkill(slotIndex);
            OnEquipSkill?.Invoke(skill);
        }

        public void UnEquipSkill(string skillResID, int slotIndex)
        {
            SkillBase skill = mAllSkills[skillResID];
            OnUnEquipSkill?.Invoke(skill);
            SkillSlots[slotIndex].OnUnEquipedSkill();
            SkillSlots[slotIndex] = null;
        }
        public int FindEmptySkillSlotIndex()
        {
            for (int i = 0; i < SkillSlots.Length; ++i)
            {
                if (SkillSlots[i] == null)
                    return i;
            }
            return -1;
        }

    }
}