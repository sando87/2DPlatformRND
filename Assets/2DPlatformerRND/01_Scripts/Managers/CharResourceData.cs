using UnityEngine;

namespace PahlBit
{
    public class CharResourceTable : DatabaseCSV<CharResourceData> { }

    [System.Serializable]
    public class CharResourceData : ICSVFormat
    {
        public readonly string PlayerID;
        public readonly string DisplayName;
        public readonly string Desc;

        public readonly double Health;
        public readonly double HealthPerLv;
        public readonly double HealthPerPoint;
        public readonly double Attack;
        public readonly double AttackPerLv;
        public readonly double AttackPerPoint;
        public readonly double Defence;
        public readonly double DefencePerLv;
        public readonly double DefencePerPoint;
        public readonly double Mana;
        public readonly double ManaPerLv;
        public readonly double ManaPerPoint;


        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ToID(PlayerID); } } // 데이터 접근을 위한 id값
        public static long ToID(string nameID) { return nameID.GetHashCode(); }

    }

}