using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PahlBit
{
    public class ItemResourceTable : DatabaseCSV<ItemResourceData> { }

    [System.Serializable]
    public class ItemResourceData : ICSVFormat
    {
        public readonly string ItemID;
        public readonly string DisplayName;
        public readonly string Desc;
        public readonly string PrefabName;

        public readonly string HealthUp;
        public readonly double HealthRegen;
        public readonly string ManaUp;
        public readonly double ManaRegen;
        public readonly string AttackUp;
        public readonly string DefenceUp;
        public readonly string MoveSpeedUp;
        public readonly string AttackSpeedUp;
        public readonly string CooltimeDown;
        public readonly string ShieldAdd;
        public readonly double ShieldRegen;
        public readonly string FireResist;
        public readonly string IceResist;
        public readonly string LightningResist;
        public readonly string PosionResist;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ToID(ItemID); } } // 데이터 접근을 위한 id값
        public static long ToID(string nameID) { return nameID.GetHashCode(); }

        public BaseStepPair HealthUpPair { get; private set; }
        public BaseStepPair ManaUpPair { get; private set; }
        public BaseStepPair AttackUpPair { get; private set; }
        public BaseStepPair DefenceUpPair { get; private set; }

        public Percent MoveSpeedUpPercent { get; private set; }
        public Percent AttackSpeedUpPercent { get; private set; }
        public Percent CooltimeDownPercent { get; private set; }

        public RangeType ShieldAddRange { get; private set; }

        public Percent FireResistPercent { get; private set; }
        public Percent IceResistPercent { get; private set; }
        public Percent LightningResistPercent { get; private set; }
        public Percent PosionResistPercent { get; private set; }

        void ICSVFormat.OnLoad()
        {
            HealthUpPair = BaseStepPair.Parse(HealthUp);
            ManaUpPair = BaseStepPair.Parse(ManaUp);
            AttackUpPair = BaseStepPair.Parse(AttackUp);
            DefenceUpPair = BaseStepPair.Parse(DefenceUp);

            MoveSpeedUpPercent = Percent.Parse(MoveSpeedUp);
            AttackSpeedUpPercent = Percent.Parse(AttackSpeedUp);
            CooltimeDownPercent = Percent.Parse(CooltimeDown);

            ShieldAddRange = RangeType.Parse(ShieldAdd);

            FireResistPercent = Percent.Parse(FireResist);
            IceResistPercent = Percent.Parse(IceResist);
            LightningResistPercent = Percent.Parse(LightningResist);
            PosionResistPercent = Percent.Parse(PosionResist);
        }
    }

}