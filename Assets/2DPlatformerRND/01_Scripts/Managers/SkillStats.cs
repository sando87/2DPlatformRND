
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace PahlBit
{
    public class SkillStats
    {
        public double Attack;
        public double ManaUse;
        public double Cooltime;
        public double ProjectileCount;
        public double AttackRange;
        public double SplashRange;
        public double Duration;
        public double Interval;

        public static SkillStats operator *(SkillStats stat, ItemStats option)
        {
            SkillStats result = new SkillStats();
            // result.Health = stat.Health * (1 + option.HealthUp.ToDouble()) + option.HealthRegen;
            // result.Mana = stat.Mana * (1 + option.ManaUp.ToDouble()) + option.ManaRegen;
            // result.Attack = stat.Attack * (1 + option.AttackUp.ToDouble());
            // result.Defence = stat.Defence * (1 + option.DefenceUp.ToDouble());
            // result.MoveSpeed = stat.MoveSpeed * (1 + option.MoveSpeedUp.ToDouble());
            // result.AttackSpeed = stat.AttackSpeed * (1 + option.AttackSpeedUp.ToDouble());
            // result.Shield = stat.Shield + option.ShieldAdd + option.ShieldRegen;
            return result;
        }
    }
}