
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace PahlBit
{
    [System.Serializable]
    public class SkillStats
    {
        public float Attack;
        public float ManaUse;
        public float Cooltime;
        public float ProjectileCount;
        public float ProjectileSpeed;
        public float ProjectileDistance;
        public float AttackRange;
        public float SplashRange;
        public float Duration;
        public float Interval;

        public static SkillStats operator *(SkillStats stat, SpecOption option)
        {
            SkillStats result = new SkillStats();
            result.Attack = stat.Attack * option.AttackUp;
            result.ManaUse = stat.ManaUse;
            result.Cooltime = stat.Cooltime * option.CooltimeDown;
            result.ProjectileCount = stat.ProjectileCount;
            result.ProjectileSpeed = stat.ProjectileSpeed;
            result.ProjectileDistance = stat.ProjectileDistance;
            result.AttackRange = stat.AttackRange;
            result.SplashRange = stat.SplashRange;
            result.Duration = stat.Duration;
            result.Interval = stat.Interval;
            return result;
        }
    }
}