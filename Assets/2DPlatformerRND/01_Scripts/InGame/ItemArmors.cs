using UnityEngine;

namespace PahlBit
{
    [System.Serializable]
    public class ItemArmors : ICSVFormat
    {
        public string NameID;
        public string DisplayName;
        public string Desc;

        public string Health;
        public string HealthRate;
        public string Defence;
        public string Shield;
        public string ShieldRegen;
        public string FireResist;
        public string IceResist;
        public string LightningResist;
        public string PosionResist;


        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public string ID { get { return NameID; } } // 데이터 접근을 위한 id값

    }

}