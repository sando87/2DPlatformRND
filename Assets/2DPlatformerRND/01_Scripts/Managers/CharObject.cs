using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class CharObject : MonoBehaviour
    {
        [SerializeField] float _MoveSpeed = 5f;
        [SerializeField] float _AttackSpeed = 1f;

        [Button]
        void UpdateMotionSpeed()
        {
            this.ExGetBase().AnimHelper.SetParamFloat(AnimatorParams.MotionSpeed, _AttackSpeed);
        }

        [SerializeField]
        [Dropdown("IDList")]
        string _ID = "";
        List<string> IDList { get => CharResourceTable.Instance.GetAllInfo().Select(info => info.CharacterID).ToList(); }

        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        public CharSaveData SaveData { get; private set; } = null;
        public CharResourceData ResourceData { get; private set; } = null;

        [field: SerializeField]
        public CharStats BaseStats { get; private set; } = new CharStats();
        [field: SerializeField]
        public CharStats TotalStats { get; private set; } = new CharStats();

        void Awake()
        {
            Init(_ID);
        }

        public void Init(string statsID)
        {
            ResourceData = CharResourceTable.Instance.GetInfo(statsID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;
            SaveData = userSaveData.Characters[charID].Stats;

            UpdateBasicStat();
            UpdateTotalStat();
        }

        void UpdateBasicStat()
        {
            int currentLevelIndex = CharRoot.Exp.CurrentLevelIdx;

            BaseStats.Attack = ResourceData._Attack.GetValueByBoth(SaveData.AttackPoint, currentLevelIndex);
            BaseStats.Defence = ResourceData._Defence.GetValueByBoth(SaveData.DefensePoint, currentLevelIndex);
            BaseStats.Health = ResourceData._Health.GetValueByBoth(SaveData.HealthPoint, currentLevelIndex);
            BaseStats.Mana = ResourceData._Mana.GetValueByBoth(SaveData.ManaPoint, currentLevelIndex);

            BaseStats.Shield = 0;
            BaseStats.MoveSpeed = _MoveSpeed;
            BaseStats.AttackSpeed = _AttackSpeed;
        }
        void UpdateTotalStat()
        {
            ItemStats totalOption = new ItemStats();
            totalOption.Add(CharRoot.Inven.TotalItemOption);
            totalOption.Add(CharRoot.Buffs.TotalBuffOption);

            TotalStats = BaseStats * totalOption;
        }

    }
}