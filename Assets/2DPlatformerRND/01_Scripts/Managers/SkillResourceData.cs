using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PahlBit
{
    public class SkillResourceTable : DatabaseCSV<SkillResourceData> { }

    [System.Serializable]
    public class SkillResourceData : ICSVFormat
    {
        public readonly string SkillID;
        public readonly string DisplayName;
        public readonly string Desc;

        public readonly string Attack;
        public readonly string ManaUse;
        public readonly string Cooltime;
        public readonly string ProjectileCount;
        public readonly string ProjectileSpeed;
        public readonly string ProjectileDistance;
        public readonly string AttackRange;
        public readonly string SplashRange;
        public readonly string Duration;
        public readonly string Interval;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ICSVFormat.ToID(SkillID); } } // 데이터 접근을 위한 id값

        public ParseValue _Attack { get; private set; }
        public ParseValue _ManaUse { get; private set; }
        public ParseValue _Cooltime { get; private set; }
        public ParseValue _ProjectileCount { get; private set; }
        public ParseValue _ProjectileSpeed { get; private set; }
        public ParseValue _ProjectileDistance { get; private set; }
        public ParseValue _AttackRange { get; private set; }
        public ParseValue _SplashRange { get; private set; }
        public ParseValue _Duration { get; private set; }
        public ParseValue _Interval { get; private set; }


        void ICSVFormat.OnLoad()
        {
            _Attack = ParseValue.Parse(Attack);
            _ManaUse = ParseValue.Parse(ManaUse);
            _Cooltime = ParseValue.Parse(Cooltime);
            _ProjectileCount = ParseValue.Parse(ProjectileCount);
            _ProjectileSpeed = ParseValue.Parse(ProjectileSpeed);
            _ProjectileDistance = ParseValue.Parse(ProjectileDistance);
            _AttackRange = ParseValue.Parse(AttackRange);
            _SplashRange = ParseValue.Parse(SplashRange);
            _Duration = ParseValue.Parse(Duration);
            _Interval = ParseValue.Parse(Interval);
        }
    }

}