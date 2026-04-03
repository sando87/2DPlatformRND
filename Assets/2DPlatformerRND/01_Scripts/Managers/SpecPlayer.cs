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

        public override float MaxHealth => BaseStats.Health * Option.HealthUp;
        public override float MaxMana => BaseStats.Mana * Option.ManaUp;
        public override float MaxShield => BaseStats.Shield + Option.ShieldAdd;

        public override float BaseAttack => BaseStats.Attack + Option.BaseAttackAdd;
        public override float PhyDefence => BaseStats.Defence * Option.DefenceUp;
        /// </summary>
        // 체력
        // 마나
        // 공격력
        // 방어력
        // 회피확률
        // 피격모션감소
        // 피격모션회복
        // 이속
        // 공속
        // 쿨타임감소
        // 쉴드
        // 크리확률
        // 크리뎀지
        // 체력재생
        // 마나재생
        // 쉴드재생
        // 레지스터 4종
        /// </summary>

        public CharSaveData SaveData { get; private set; } = null;
        public CharResourceData ResourceData { get; private set; } = null;
        public CharStats BaseStats { get; private set; } = null;

        BaseObject mBaseObj = null;

        public void Init(int characterID, string resourceID)
        {
            mBaseObj = this.ExGetBase();

            ResourceData = CharResourceTable.Instance.GetInfo(resourceID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            SaveData = userSaveData.Characters[characterID].Stats;

            UpdateBasicStat();
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