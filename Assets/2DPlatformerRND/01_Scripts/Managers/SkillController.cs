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
        [SerializeField] Dictionary<long, SkillObject> mAllSkills = new Dictionary<long, SkillObject>();
        [SerializeField] SkillObject[] SkillSlots = null;

        [Foldout("Events")]
        public UnityEvent<SkillObject> OnEquipSkill = new UnityEvent<SkillObject>();
        [Foldout("Events")]
        public UnityEvent<SkillObject> OnUnEquipSkill = new UnityEvent<SkillObject>();

        BaseObject mBaseObj = null;

        void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
        }

        public void InitSkills(int characterID)
        {
            SkillObject[] allSkills = GetComponentsInChildren<SkillObject>();
            foreach (SkillObject skillObj in allSkills)
            {
                skillObj.InitSkillInfo(characterID);
                mAllSkills[skillObj.SkillID] = skillObj;

                if (!skillObj.SkillInfo.IsLearned)
                {
                    skillObj.gameObject.SetActive(false);
                    continue;
                }

                if (skillObj.SkillInfo.IsEquipped)
                {
                    SkillSlots[skillObj.SkillInfo.PositionIndex] = skillObj;
                }
            }
        }

        public void Update()
        {
            foreach (SkillObject skillObject in SkillSlots)
            {
                if (skillObject != null)
                    skillObject.UpdateSkill();
            }
        }


        public void LearnNewSkill(long skillResID)
        {
            SkillObject skill = mAllSkills[skillResID];
            skill.gameObject.SetActive(true);
            skill.OnLearnSkill();
            GameSystem.DoSave_UserSaveData();
        }
        public void LevelupSkill(long skillResID)
        {
            SkillObject skill = mAllSkills[skillResID];
            skill.OnLevelupSkill();
            GameSystem.DoSave_UserSaveData();
        }

        public void EquipSkill(long skillResID, int slotIndex)
        {
            SkillObject skill = mAllSkills[skillResID];
            SkillSlots[slotIndex] = skill;
            SkillSlots[slotIndex].OnEquipSkill(slotIndex);
            OnEquipSkill?.Invoke(skill);

            GameSystem.DoSave_UserSaveData();
        }

        public void UnEquipSkill(long skillResID, int slotIndex)
        {
            SkillObject skill = mAllSkills[skillResID];
            OnUnEquipSkill?.Invoke(skill);
            SkillSlots[slotIndex].OnUnEquipSkill();
            SkillSlots[slotIndex] = null;

            GameSystem.DoSave_UserSaveData();
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