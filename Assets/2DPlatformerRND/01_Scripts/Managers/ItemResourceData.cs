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
        public readonly string HealthRegen;
        public readonly string ManaUp;
        public readonly string ManaRegen;
        public readonly string AttackUp;
        public readonly string DefenceUp;
        public readonly string MoveSpeedUp;
        public readonly string AttackSpeedUp;
        public readonly string CooltimeDown;
        public readonly string ShieldAdd;
        public readonly string ShieldRegen;
        public readonly string FireResist;
        public readonly string IceResist;
        public readonly string LightningResist;
        public readonly string PosionResist;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ICSVFormat.ToID(ItemID); } } // 데이터 접근을 위한 id값

        public ParseValue _HealthUp { get; private set; }
        public ParseValue _HealthRegen { get; private set; }
        public ParseValue _ManaUp { get; private set; }
        public ParseValue _ManaRegen { get; private set; }
        public ParseValue _AttackUp { get; private set; }
        public ParseValue _DefenceUp { get; private set; }
        public ParseValue _MoveSpeedUp { get; private set; }
        public ParseValue _AttackSpeedUp { get; private set; }
        public ParseValue _CooltimeDown { get; private set; }
        public ParseValue _ShieldAdd { get; private set; }
        public ParseValue _ShieldRegen { get; private set; }
        public ParseValue _FireResist { get; private set; }
        public ParseValue _IceResist { get; private set; }
        public ParseValue _LightningResist { get; private set; }
        public ParseValue _PosionResist { get; private set; }

        void ICSVFormat.OnLoad()
        {
            _HealthUp = ParseValue.Parse(HealthUp);
            _HealthRegen = ParseValue.Parse(HealthRegen);
            _ManaUp = ParseValue.Parse(ManaUp);
            _ManaRegen = ParseValue.Parse(ManaRegen);
            _AttackUp = ParseValue.Parse(AttackUp);
            _DefenceUp = ParseValue.Parse(DefenceUp);
            _MoveSpeedUp = ParseValue.Parse(MoveSpeedUp);
            _AttackSpeedUp = ParseValue.Parse(AttackSpeedUp);
            _CooltimeDown = ParseValue.Parse(CooltimeDown);
            _ShieldAdd = ParseValue.Parse(ShieldAdd);
            _ShieldRegen = ParseValue.Parse(ShieldRegen);
            _FireResist = ParseValue.Parse(FireResist);
            _IceResist = ParseValue.Parse(IceResist);
            _LightningResist = ParseValue.Parse(LightningResist);
            _PosionResist = ParseValue.Parse(PosionResist);
        }
    }

}