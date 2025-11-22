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

        public BaseStepPair _HealthUp { get; private set; }
        public BaseStepPair _ManaUp { get; private set; }
        public BaseStepPair _AttackUp { get; private set; }
        public BaseStepPair _DefenceUp { get; private set; }
        public Percent _MoveSpeedUp { get; private set; }
        public Percent _AttackSpeedUp { get; private set; }
        public Percent _CooltimeDown { get; private set; }
        public Percent _FireResist { get; private set; }
        public Percent _IceResist { get; private set; }
        public Percent _LightningResist { get; private set; }
        public Percent _PosionResist { get; private set; }

        void ICSVFormat.OnLoad()
        {
            _HealthUp = BaseStepPair.Parse(HealthUp);
            _ManaUp = BaseStepPair.Parse(ManaUp);
            _AttackUp = BaseStepPair.Parse(AttackUp);
            _DefenceUp = BaseStepPair.Parse(DefenceUp);
            
            _MoveSpeedUp = Percent.Parse(MoveSpeedUp);
            _AttackSpeedUp = Percent.Parse(AttackSpeedUp);
            _CooltimeDown = Percent.Parse(CooltimeDown);
            _FireResist = Percent.Parse(FireResist);
            _IceResist = Percent.Parse(IceResist);
            _LightningResist = Percent.Parse(LightningResist);
            _PosionResist = Percent.Parse(PosionResist);
        }
    }

}