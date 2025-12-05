using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField] SkillObject[] SkillSlots;

        [Foldout("Events")]
        public UnityEvent<SkillObject> OnEquipSkill = new UnityEvent<SkillObject>();
        [Foldout("Events")]
        public UnityEvent<SkillObject> OnUnEquipSkill = new UnityEvent<SkillObject>();

        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        BaseObject mBaseObj = null;

        void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
        }

        void Start()
        {
            InitSkillsFromInspector();

            // LoadSkillsFromSaveData();
        }

        // 인스펙터에 세팅된 상태로 스킬 초기화 한다(임시코드 나중에는 세이브정보로 초기화 해야함)
        void InitSkillsFromInspector()
        {
            for (int i = 0; i < SkillSlots.Length; ++i)
            {
                LearnNewSkill(SkillSlots[i].ResourceID);
            }

            for (int i = 0; i < SkillSlots.Length; ++i)
            {
                EquipSkill(SkillSlots[i], i);
            }
        }

        public void LearnNewSkill(long skillResID)
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;
            var savedSkills = saveData.Characters[charID].Skills;
            if (!savedSkills.ContainsKey(skillResID))
            {
                SkillSaveData skillSaveData = new SkillSaveData();
                skillSaveData.ResourceID = skillResID;
                skillSaveData.IsEquipped = false;
                skillSaveData.PositionIndex = -1;
                skillSaveData.Level = 1;
                savedSkills[skillResID] = skillSaveData;
                SaveFileManager<UserSaveData>.Save(saveData);
            }
        }

        // 세이브데이터로부터 스킬 초기화 한다
        void LoadSkillsFromSaveData()
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;
            var savedSkills = saveData.Characters[charID].Skills;
            foreach (var pair in savedSkills)
            {
                SkillSaveData skillSaveData = pair.Value;
                if (skillSaveData.IsEquipped)
                {
                    SkillResourceData skillResData = SkillResourceTable.Instance.GetInfo(skillSaveData.ResourceID);
                    SkillObject skillObject = Instantiate(skillResData.SkillPrefab, transform);
                    skillObject.LoadSkillData();
                    SkillSlots[skillSaveData.PositionIndex] = skillObject;
                }
            }
        }

        public void Update()
        {
            foreach (SkillObject skillObject in SkillSlots)
            {
                skillObject.UpdateSkill();
            }
        }

        public void EquipSkill(SkillObject skill, int slotIndex)
        {
            // if (SkillSlots[slotIndex] != null)
            // {
            //     UnEquipSkill(slotIndex);
            // }

            SkillSlots[slotIndex] = skill;
            SkillSlots[slotIndex].LoadSkillData();
            SkillSlots[slotIndex].IsEquipped = true;
            SkillSlots[slotIndex].PositionIndex = slotIndex;
            SkillSlots[slotIndex].OnEquipSkill();
            OnEquipSkill?.Invoke(skill);

            GameSystem.DoSave_UserSaveData();
        }

        public void UnEquipSkill(int slotIndex)
        {
            if (SkillSlots[slotIndex] == null)
                return;

            OnUnEquipSkill?.Invoke(SkillSlots[slotIndex]);
            SkillSlots[slotIndex].OnUnEquipSkill();
            SkillSlots[slotIndex].IsEquipped = false;
            SkillSlots[slotIndex].PositionIndex = -1;
            SkillSlots[slotIndex] = null;

            GameSystem.DoSave_UserSaveData();
        }

    }
}