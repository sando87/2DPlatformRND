
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
                FireResist = a.FireResist - b.FireResist,
                IceResist = a.IceResist - b.IceResist,
                LightningResist = a.LightningResist - b.LightningResist,
                PosionResist = a.PosionResist - b.PosionResist,
            };
        }
    }

}