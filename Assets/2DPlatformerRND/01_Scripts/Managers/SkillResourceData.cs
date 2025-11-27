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

        public readonly double Attack;
        public readonly double AttackPerLv;
        public readonly double ManaUse;
        public readonly double ManaUsePerLv;
        public readonly double Cooltime;
        public readonly double CooltimeDownPerLv;
        public readonly double ProjectileCount;
        public readonly double ProjectileCountPerLv;
        public readonly double AttackRange;
        public readonly double AttackRangePerLv;
        public readonly double SplashRange;
        public readonly double SplashRangePerLv;
        public readonly double Duration;
        public readonly double DurationPerLv;
        public readonly double Interval;
        public readonly double IntervalPerLv;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ToID(SkillID); } } // 데이터 접근을 위한 id값
        public static long ToID(string nameID) { return nameID.GetHashCode(); }
    }

}