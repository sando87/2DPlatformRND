
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace PahlBit
{
    [System.Serializable]
    public class CharStats
    {
        public float Health;
        public float Mana;
        public float Shield;
        public float Attack;
        public float Defence;
        public float MoveSpeed;
        public float AttackSpeed;

        public static CharStats operator *(CharStats stat, ItemStats option)
        {
            CharStats result = new CharStats();

            result.Health = stat.Health * option.HealthUp;
            result.Mana = stat.Mana * option.ManaUp;
            result.Shield = option.ShieldAdd;
            result.Attack = stat.Attack * option.AttackUp;
            result.Defence = stat.Defence * option.DefenceUp;
            result.MoveSpeed = stat.MoveSpeed * option.MoveSpeedUp;
            result.AttackSpeed = stat.AttackSpeed * option.AttackSpeedUp;

            return result;
        }
    }
}