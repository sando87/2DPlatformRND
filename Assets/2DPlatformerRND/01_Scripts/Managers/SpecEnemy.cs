using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SpecEnemy : SpecBase
    {
        public override float MaxHealth => TotalStats.Health;

        public override float BaseAttack => BaseStats.Attack + Option.BaseAttackAdd;
        public override float PhyDefence => BaseStats.Defence * Option.DefenceUp;

        public EnemyResourceData ResourceData { get; private set; } = null;
        public EnemyStats BaseStats { get; private set; } = null;

        [field: SerializeField]
        public EnemyStats TotalStats { get; private set; } = null;

        public void InitData(string resourceID)
        {
            ResourceData = EnemyResourceTable.Instance.GetInfo(resourceID);
            UpdateBasicStats();
        }
        void UpdateBasicStats()
        {
            BaseStats = new EnemyStats();

            BaseStats.Health = ResourceData._Health.GetValue();
            BaseStats.Attack = ResourceData._Attack.GetValue();
            BaseStats.Defence = ResourceData._Defence.GetValue();
            BaseStats.MoveSpeed = ResourceData._MoveSpeed.GetValue();
            BaseStats.AttackSpeed = ResourceData._AttackSpeed.GetValue();
            BaseStats.Cooltime = ResourceData._Cooltime.GetValue();
            BaseStats.DetectRange = ResourceData._DetectRange.GetValue();
            BaseStats.AttackRange = ResourceData._AttackRange.GetValue();
            BaseStats.ItemDrop = (Percent)ResourceData._ItemDrop.GetValue();
            BaseStats.GoldOnDeath = (int)ResourceData._GoldOnDeath.GetValueInRange(MyUtils.RandomRate());
            BaseStats.ExpOnDeath = ResourceData._ExpOnDeath.GetValue();
        }
    }
}