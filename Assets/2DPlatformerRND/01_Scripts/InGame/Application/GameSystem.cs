using UnityEngine;

namespace PahlBit
{
    public static class GameSystem
    {
        public static BuffOption CalculateOption(ItemData data)
        {
            BuffOption option = new BuffOption();
            GameDataItem resourceData = TableItem.Instance.GetInfo(data.ResourceID);;
            int point = data.LevelIndex;
            System.Random ran = new System.Random(data.RandomSeed);

            option.HealthUp = resourceData.HealthUpPair.GetValue(point);
            option.HealthRegen = resourceData.HealthRegen;
            option.MoveSpeedUp = resourceData.MoveSpeedUpPercent;
            option.ShieldAdd = resourceData.ShieldAddRange.GetDouble(ran.NextDouble());
            
            return option;
        }
    }
}