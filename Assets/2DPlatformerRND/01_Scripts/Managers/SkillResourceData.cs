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

        public readonly float Attack;
        public readonly float AttackPerLv;
        public readonly float ManaUse;
        public readonly float ManaUsePerLv;
        public readonly float Cooltime;
        public readonly float CooltimeDownPerLv;
        public readonly float ProjectileCount;
        public readonly float ProjectileCountPerLv;
        public readonly float AttackRange;
        public readonly float AttackRangePerLv;
        public readonly float SplashRange;
        public readonly float SplashRangePerLv;
        public readonly float Duration;
        public readonly float DurationPerLv;
        public readonly float Interval;
        public readonly float IntervalPerLv;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ToID(SkillID); } } // 데이터 접근을 위한 id값
        public static long ToID(string nameID) { return nameID.GetHashCode(); }
    }

}