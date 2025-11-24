using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class CharacterStats : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        public StatsSaveData SaveData { get; private set; } = null;
        public StatsResourceData ResourceData { get; private set; } = null;
        public StatsValue BasicStat { get; private set; } = new StatsValue();
        public StatsValue TotalStat { get; private set; } = new StatsValue();

        public void Init(long statsID)
        {
            ResourceData = StatsResourceTable.Instance.GetInfo(statsID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            int charID = CharRoot.CharacterID;
            SaveData = userSaveData.Characters[charID].Stats;

            UpdateBasicStat();
            UpdateTotalStat();
        }

        void UpdateBasicStat()
        {
            int currentLevelIndex = CharRoot.Exp.CurrentLevelIdx;

            BasicStat.Attack = ResourceData.Attack
                            + (ResourceData.AttackPerLv * currentLevelIndex)
                            + (ResourceData.AttackPerPoint * SaveData.AttackPoint);

            BasicStat.Defence = ResourceData.Defence
                            + (ResourceData.DefencePerLv * currentLevelIndex)
                            + (ResourceData.DefencePerPoint * SaveData.DefensePoint);

            BasicStat.Health = ResourceData.Health
                            + (ResourceData.HealthPerLv * currentLevelIndex)
                            + (ResourceData.HealthPerPoint * SaveData.HealthPoint);

            BasicStat.Mana = ResourceData.Mana
                            + (ResourceData.ManaPerLv * currentLevelIndex)
                            + (ResourceData.ManaPerPoint * SaveData.ManaPoint);

            BasicStat.Shield = 0;
            BasicStat.MoveSpeed = 5;
            BasicStat.AttackSpeed = 1;
        }
        void UpdateTotalStat()
        {
            StatsOption totalOption = new StatsOption();
            totalOption.Add(CharRoot.Inven.TotalItemOption);
            totalOption.Add(CharRoot.Buffs.TotalBuffOption);

            TotalStat = BasicStat * totalOption;
        }

    }
}