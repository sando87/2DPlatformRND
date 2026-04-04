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
        public float MoveSpeed => BaseStats.MoveSpeed * Option.MoveSpeedUp;
        public float AttackSpeed => BaseStats.AttackSpeed * Option.AttackSpeedUp;

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


        public void GetDisplayInfo(List<FieldData> fieldDatas)
        {
            fieldDatas.Clear();

            /// </summary>
            // 체력
            // 체력재생
            // 마나
            // 마나재생
            // 쉴드
            // 쉴드재생
            // 공격력
            // 방어력
            // 이속
            // 공속
            // 쿨타임감소
            // 크리확률
            // 크리뎀지
            // 레지스터 4종
            // 회피확률
            // 피격방어
            // 피격회복
            /// </summary>

            fieldDatas.Add(new FieldData() { Name = nameof(MaxHealth), Value = ((int)MaxHealth).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.HealthRegen), Value = ((int)Option.HealthRegen).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(MaxMana), Value = ((int)MaxMana).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.ManaRegen), Value = ((int)Option.ManaRegen).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(MaxShield), Value = ((int)MaxShield).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.ShieldRegen), Value = ((int)Option.ShieldRegen).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(BaseAttack), Value = ((int)BaseAttack).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(PhyDefence), Value = ((int)PhyDefence).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(MoveSpeed), Value = ((int)MoveSpeed).ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(AttackSpeed), Value = AttackSpeed.ToString("0.##") });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.CooltimeDown), Value = Option.CooltimeDown.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.CriticalRate), Value = Option.CriticalRate.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.CriticalAttack), Value = Option.CriticalAttack.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.FireResist), Value = Option.FireResist.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.IceResist), Value = Option.IceResist.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.LightningResist), Value = Option.LightningResist.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.PosionResist), Value = Option.PosionResist.ToString() });
        }

    }
}