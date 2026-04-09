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

        private WrapStation mWrapStationAround = null;

        public List<ItemObject> ItemsAround { get; private set; } = new List<ItemObject>();

        public Experience Exp { get; private set; }
        public ItemInventory Inven { get; private set; }
        public SkillController SkillCtrl { get; private set; }
        public SpecPlayer Spec { get; private set; }

        BaseObject mBaseObj = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();

            Exp = GetComponentInChildren<Experience>();
            Exp.Init(CharacterID);

            Inven = GetComponentInChildren<ItemInventory>();
            Inven.LoadItemsFromData(CharacterID);

            Spec = GetComponentInChildren<SpecPlayer>();
            Spec.Init(CharacterID, _ResourceID);
            Spec.LinkOption(Inven.TotalItemOption);
            Spec.LinkOption(mBaseObj.Buffs.TotalBuffOption);

            SkillCtrl = GetComponentInChildren<SkillController>();
            SkillCtrl.InitSkills(CharacterID);
        }

        void Start()
        {
            mBaseObj.Interactor.OnInteractEnter.AddListener(OnColliderEnter);
            mBaseObj.Interactor.OnInteractLeave.AddListener(OnColliderLeave);
        }

        void OnColliderEnter(Collider2D col)
        {
            ItemObject itemObj = col.ExGetCompInBase<ItemObject>();
            if (itemObj != null)
            {
                ItemsAround.Add(itemObj);
            }

            WrapStation wrapStation = col.ExGetCompInBase<WrapStation>();
            if (wrapStation != null)
            {
                mWrapStationAround = wrapStation;
            }

        }
        void OnColliderLeave(Collider2D col)
        {
            ItemObject itemObj = col.ExGetCompInBase<ItemObject>();
            if (itemObj != null)
            {
                ItemsAround.Remove(itemObj);
            }

            WrapStation wrapStation = col.ExGetCompInBase<WrapStation>();
            if (wrapStation != null)
            {
                mWrapStationAround = null;
            }
        }

        void Update()
        {
            if (mBaseObj.Input.JustPressed(PlayerUnitInputType.PotionA))
            {
                if (Inven.CurrentLifePotionCount > 0)
                {
                    Inven.CurrentLifePotionCount--;
                    mBaseObj.Health.Heal(30);
                }
            }

            if (mBaseObj.Input.JustPressed(PlayerUnitInputType.PotionB))
            {
                if (Inven.CurrentManaPotionCount > 0)
                {
                    Inven.CurrentManaPotionCount--;
                    mBaseObj.Health.RestoreMana(10);
                }
            }

            if (mWrapStationAround != null && mBaseObj.Input.JustPressed(PlayerUnitInputType.Move))
            {
                if (mBaseObj.Input.MoveY < 0)
                {
                    InGameManager.Instance.Engine.DoWarpStation(mWrapStationAround.DestScene, mWrapStationAround.DestWarpID);
                }
            }
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