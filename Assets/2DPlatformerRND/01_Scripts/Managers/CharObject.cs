using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class CharObject : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        public CharSaveData SaveData { get; private set; } = null;
        public CharResourceData ResourceData { get; private set; } = null;
        public CharStats BaseStats { get; private set; } = new CharStats();
        public CharStats TotalStats { get; private set; } = new CharStats();

        public void Init(long statsID)
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

            BaseStats.Attack = ResourceData.Attack
                            + (ResourceData.AttackPerLv * currentLevelIndex)
                            + (ResourceData.AttackPerPoint * SaveData.AttackPoint);

            BaseStats.Defence = ResourceData.Defence
                            + (ResourceData.DefencePerLv * currentLevelIndex)
                            + (ResourceData.DefencePerPoint * SaveData.DefensePoint);

            BaseStats.Health = ResourceData.Health
                            + (ResourceData.HealthPerLv * currentLevelIndex)
                            + (ResourceData.HealthPerPoint * SaveData.HealthPoint);

            BaseStats.Mana = ResourceData.Mana
                            + (ResourceData.ManaPerLv * currentLevelIndex)
                            + (ResourceData.ManaPerPoint * SaveData.ManaPoint);

            BaseStats.Shield = 0;
            BaseStats.MoveSpeed = 5;
            BaseStats.AttackSpeed = 1;
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