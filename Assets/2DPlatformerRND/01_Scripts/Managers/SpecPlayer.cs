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

        public override float MaxHealth => TotalStats.Health;
        public override float MaxMana => TotalStats.Mana;
        public override float MaxShield => TotalStats.Shield;

        public CharSaveData SaveData { get; private set; } = null;
        public CharResourceData ResourceData { get; private set; } = null;
        public CharStats BaseStats { get; private set; } = null;

        [field: SerializeField]
        public CharStats TotalStats { get; private set; } = null;

        BaseObject mBaseObj = null;

        public void Init(int characterID, string resourceID)
        {
            mBaseObj = this.ExGetBase();

            ResourceData = CharResourceTable.Instance.GetInfo(resourceID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            SaveData = userSaveData.Characters[characterID].Stats;

            UpdateBasicStat();

            Option = mBaseObj.PlayerObj.Inven.TotalItemOption;
        }

        void UpdateBasicStat()
        {
            int currentLevelIndex = GameSystem.CurrentExpToLevel(SaveData.CurrentExp);

            BaseStats = new CharStats();

            BaseStats.Attack = ResourceData._Attack.GetValueByBoth(SaveData.AttackPoint, currentLevelIndex);
            BaseStats.Defence = ResourceData._Defence.GetValueByBoth(SaveData.DefensePoint, currentLevelIndex);
            BaseStats.Health = ResourceData._Health.GetValueByBoth(SaveData.HealthPoint, currentLevelIndex);
            BaseStats.Mana = ResourceData._Mana.GetValueByBoth(SaveData.ManaPoint, currentLevelIndex);

            BaseStats.Shield = 0;
            BaseStats.MoveSpeed = _MoveSpeed;
            BaseStats.AttackSpeed = _AttackSpeed;
        }

    }
}