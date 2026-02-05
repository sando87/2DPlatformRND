
namespace PahlBit
{
    [System.Serializable]
    public class SpecOption
    {
        public Percent HealthUp;
        public float HealthRegen;
        public Percent ManaUp;
        public float ManaRegen;
        public Percent AttackUp;
        public Percent DefenceUp;
        public Percent MoveSpeedUp;
        public Percent AttackSpeedUp;
        public Percent CooltimeDown;
        public float ShieldAdd;
        public float ShieldRegen;
        public Percent CriticalRate;
        public Percent CriticalAttack;
        public float ProjectileCountUp;
        public Percent ProjectileSpeedUp;
        public Percent SplashRangeUp;
        public Percent DurationUp;
        public Percent FireResist;
        public Percent IceResist;
        public Percent LightningResist;
        public Percent PosionResist;

        // ----- += 연산 메서드 -----
        public void Add(SpecOption other)
        {
            HealthUp += other.HealthUp;
            HealthRegen += other.HealthRegen;
            ManaUp += other.ManaUp;
            ManaRegen += other.ManaRegen;
            AttackUp += other.AttackUp;
            DefenceUp += other.DefenceUp;
            MoveSpeedUp += other.MoveSpeedUp;
            AttackSpeedUp += other.AttackSpeedUp;
            CooltimeDown += other.CooltimeDown;
            ShieldAdd += other.ShieldAdd;
            ShieldRegen += other.ShieldRegen;
            CriticalRate += other.CriticalRate;
            CriticalAttack += other.CriticalAttack;
            ProjectileCountUp += other.ProjectileCountUp;
            ProjectileSpeedUp += other.ProjectileSpeedUp;
            SplashRangeUp += other.SplashRangeUp;
            DurationUp += other.DurationUp;
            FireResist += other.FireResist;
            IceResist += other.IceResist;
            LightningResist += other.LightningResist;
            PosionResist += other.PosionResist;
        }

        // ----- -= 연산 메서드 -----
        public void Subtract(SpecOption other)
        {
            HealthUp -= other.HealthUp;
            HealthRegen -= other.HealthRegen;
            ManaUp -= other.ManaUp;
            ManaRegen -= other.ManaRegen;
            AttackUp -= other.AttackUp;
            DefenceUp -= other.DefenceUp;
            MoveSpeedUp -= other.MoveSpeedUp;
            AttackSpeedUp -= other.AttackSpeedUp;
            CooltimeDown -= other.CooltimeDown;
            ShieldAdd -= other.ShieldAdd;
            ShieldRegen -= other.ShieldRegen;
            CriticalRate -= other.CriticalRate;
            CriticalAttack -= other.CriticalAttack;
            ProjectileCountUp -= other.ProjectileCountUp;
            ProjectileSpeedUp -= other.ProjectileSpeedUp;
            SplashRangeUp -= other.SplashRangeUp;
            DurationUp -= other.DurationUp;
            FireResist -= other.FireResist;
            IceResist -= other.IceResist;
            LightningResist -= other.LightningResist;
            PosionResist -= other.PosionResist;
        }

        public static SpecOption operator +(SpecOption a, SpecOption b)
        {
            return new SpecOption
            {
                HealthUp = a.HealthUp + b.HealthUp,
                HealthRegen = a.HealthRegen + b.HealthRegen,
                ManaUp = a.ManaUp + b.ManaUp,
                ManaRegen = a.ManaRegen + b.ManaRegen,
                AttackUp = a.AttackUp + b.AttackUp,
                DefenceUp = a.DefenceUp + b.DefenceUp,
                MoveSpeedUp = a.MoveSpeedUp + b.MoveSpeedUp,
                AttackSpeedUp = a.AttackSpeedUp + b.AttackSpeedUp,
                CooltimeDown = a.CooltimeDown + b.CooltimeDown,
                ShieldAdd = a.ShieldAdd + b.ShieldAdd,
                ShieldRegen = a.ShieldRegen + b.ShieldRegen,
                CriticalRate = a.CriticalRate + b.CriticalRate,
                CriticalAttack = a.CriticalAttack + b.CriticalAttack,
                ProjectileCountUp = a.ProjectileCountUp + b.ProjectileCountUp,
                ProjectileSpeedUp = a.ProjectileSpeedUp + b.ProjectileSpeedUp,
                SplashRangeUp = a.SplashRangeUp + b.SplashRangeUp,
                DurationUp = a.DurationUp + b.DurationUp,
                FireResist = a.FireResist + b.FireResist,
                IceResist = a.IceResist + b.IceResist,
                LightningResist = a.LightningResist + b.LightningResist,
                PosionResist = a.PosionResist + b.PosionResist,
            };
        }
        public static SpecOption operator -(SpecOption a, SpecOption b)
        {
            return new SpecOption
            {
                HealthUp = a.HealthUp - b.HealthUp,
                HealthRegen = a.HealthRegen - b.HealthRegen,
                ManaUp = a.ManaUp - b.ManaUp,
                ManaRegen = a.ManaRegen - b.ManaRegen,
                AttackUp = a.AttackUp - b.AttackUp,
                DefenceUp = a.DefenceUp - b.DefenceUp,
                MoveSpeedUp = a.MoveSpeedUp - b.MoveSpeedUp,
                AttackSpeedUp = a.AttackSpeedUp - b.AttackSpeedUp,
                CooltimeDown = a.CooltimeDown - b.CooltimeDown,
                ShieldAdd = a.ShieldAdd - b.ShieldAdd,
                ShieldRegen = a.ShieldRegen - b.ShieldRegen,
                CriticalRate = a.CriticalRate - b.CriticalRate,
                CriticalAttack = a.CriticalAttack - b.CriticalAttack,
                ProjectileCountUp = a.ProjectileCountUp - b.ProjectileCountUp,
                ProjectileSpeedUp = a.ProjectileSpeedUp - b.ProjectileSpeedUp,
                SplashRangeUp = a.SplashRangeUp - b.SplashRangeUp,
                DurationUp = a.DurationUp - b.DurationUp,
                FireResist = a.FireResist - b.FireResist,
                IceResist = a.IceResist - b.IceResist,
                LightningResist = a.LightningResist - b.LightningResist,
                PosionResist = a.PosionResist - b.PosionResist,
            };
        }
    }

}