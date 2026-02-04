using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace PahlBit
{
    public class PlayerMain : MonoBehaviour
    {
        [SerializeField] int _CharacterID = 1;
        private int mCharacterID = -1;
        public int CharacterID => GetCharacterData();

        [SerializeField]
        [Dropdown(nameof(IDList))]
        string _ResourceID = "";
        List<string> IDList { get => CharResourceTable.Instance.GetAllInfo().Select(info => info.CharacterID).ToList(); }

        public Experience Exp { get; private set; }
        public ItemInventory Inven { get; private set; }
        public SkillController SkillCtrl { get; private set; }
        public CharObject Stats { get; private set; }

        BaseObject mBaseObj = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();

            Exp = GetComponentInChildren<Experience>();
            Exp.Init(CharacterID);

            Inven = GetComponentInChildren<ItemInventory>();
            Inven.LoadItemsFromData(CharacterID);

            SkillCtrl = GetComponentInChildren<SkillController>();
            SkillCtrl.InitSkills(CharacterID);

            Stats = GetComponentInChildren<CharObject>();
            Stats.Init(CharacterID, _ResourceID);
        }
        void Start()
        {
            // mBaseObj.Health.InitHealth(mSpec.TotalStats.Health, mSpec.TotalStats.Mana, mSpec.TotalStats.Shield);
        }


        int GetCharacterData()
        {
            if (mCharacterID >= 0)
            {
                return mCharacterID;
            }

            UserSaveData userData = SaveFileManager<UserSaveData>.Load();
            if (userData.Characters.ContainsKey(_CharacterID))
            {
                mCharacterID = _CharacterID;
            }
            else
            {
                userData.Characters[_CharacterID] = new CharacterSaveData();
                mCharacterID = _CharacterID;
                SaveFileManager<UserSaveData>.Save(userData);
            }

            return mCharacterID;
        }
    }
}