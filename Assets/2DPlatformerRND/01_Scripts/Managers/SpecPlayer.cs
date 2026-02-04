using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SpecPlayer : SpecBase
    {
        [SerializeField] float _MoveSpeed = 5f;
        [SerializeField] float _AttackSpeed = 1f;

        public override SpecOption Option => TotalOption;

        public CharResourceData ResourceData { get; private set; } = null;
        public CharSaveData SaveData { get; private set; } = null;
        public CharStats BaseStats { get; private set; } = null;
        public CharStats TotalStats { get; private set; } = null;
        public SpecOption TotalOption { get; private set; } = null;

        private BaseObject mBaseObj = null;
        private ItemInventory mInven = null;
        private BuffController mBuff = null;


        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mInven = mBaseObj.GetComponentInChildren<ItemInventory>();
            mBuff = mBaseObj.GetComponentInChildren<BuffController>();
        }

        public void Init(int characterID, string resourceID)
        {
            ResourceData = CharResourceTable.Instance.GetInfo(resourceID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            SaveData = userSaveData.Characters[characterID].Stats;

            UpdateBasicStat();
            UpdateOption();
        }

        void UpdateBasicStat()
        {
            int currentLevelIndex = GameSystem.CurrentExpToLevel(SaveData.CurrentExp);

            BaseStats.Attack = ResourceData._Attack.GetValueByBoth(SaveData.AttackPoint, currentLevelIndex);
            BaseStats.Defence = ResourceData._Defence.GetValueByBoth(SaveData.DefensePoint, currentLevelIndex);
            BaseStats.Health = ResourceData._Health.GetValueByBoth(SaveData.HealthPoint, currentLevelIndex);
            BaseStats.Mana = ResourceData._Mana.GetValueByBoth(SaveData.ManaPoint, currentLevelIndex);

            BaseStats.Shield = 0;
            BaseStats.MoveSpeed = _MoveSpeed;
            BaseStats.AttackSpeed = _AttackSpeed;
        }
        void UpdateOption()
        {
            TotalOption = new SpecOption();
            TotalOption.Add(mInven.TotalItemOption);
            TotalOption.Add(mBuff.TotalBuffOption);

            TotalStats = BaseStats * mInven.TotalItemOption;
        }

    }
}