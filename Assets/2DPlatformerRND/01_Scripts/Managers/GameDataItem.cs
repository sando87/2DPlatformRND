using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PahlBit
{
    public class TableItem : DatabaseCSV<GameDataItem> { }

    [System.Serializable]
    public class GameDataItem : ICSVFormat
    {
        public readonly string ItemID;
        public readonly string DisplayName;
        public readonly string Desc;

        public readonly string HealthUp;
        public readonly double HealthRegen;
        public readonly string ManaUp;
        public readonly double ManaRegen;
        public readonly string AttackUp;
        public readonly string DefenceUp;
        public readonly string MoveSpeedUp;
        public readonly string AttackSpeedUp;
        public readonly string CooltimeDown;
        public readonly double ShieldAdd;
        public readonly double ShieldRegen;
        public readonly string FireResist;
        public readonly string IceResist;
        public readonly string LightningResist;
        public readonly string PosionResist;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ToID(ItemID); } } // 데이터 접근을 위한 id값
        public static long ToID(string nameID) { return nameID.GetHashCode(); }

        public Percent pHealthUp { get; private set; }
        public Percent pManaUp { get; private set; }
        public Percent pAttackUp { get; private set; }
        public Percent pDefenceUp { get; private set; }
        public Percent pMoveSpeedUp { get; private set; }
        public Percent pAttackSpeedUp { get; private set; }
        public Percent pCooltimeDown { get; private set; }
        public Percent pFireResist { get; private set; }
        public Percent pIceResist { get; private set; }
        public Percent pLightningResist { get; private set; }
        public Percent pPosionResist { get; private set; }

        void ICSVFormat.OnLoad()
        {
            pHealthUp = Percent.Parse(HealthUp);
            pManaUp = Percent.Parse(ManaUp);
            pAttackUp = Percent.Parse(AttackUp);
            pDefenceUp = Percent.Parse(DefenceUp);
            pMoveSpeedUp = Percent.Parse(MoveSpeedUp);
            pAttackSpeedUp = Percent.Parse(AttackSpeedUp);
            pCooltimeDown = Percent.Parse(CooltimeDown);
            pFireResist = Percent.Parse(FireResist);
            pIceResist = Percent.Parse(IceResist);
            pLightningResist = Percent.Parse(LightningResist);
            pPosionResist = Percent.Parse(PosionResist);
        }
    }

}