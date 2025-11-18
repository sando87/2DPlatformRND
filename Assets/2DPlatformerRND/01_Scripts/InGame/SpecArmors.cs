using UnityEngine;

[System.Serializable]
public class SpecArmors : ICSVFormat
{
    public readonly string NameID;
    public readonly string DisplayName;
    public readonly string Desc;
    public readonly float Defence;

    public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
    public string ID { get { return NameID; } } // 데이터 접근을 위한 id값

}

