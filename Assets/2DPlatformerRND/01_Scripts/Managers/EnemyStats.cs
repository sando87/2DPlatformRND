
namespace PahlBit
{
    [System.Serializable]
    public class EnemyStats
    {
        public float Health;
        public float Attack;
        public float Defence;
        public float MoveSpeed;
        public float AttackSpeed;
        public float Cooltime;
        public float DetectRange;
        public float AttackRange;
        public Percent ItemDrop;
        public int GoldOnDeath;
        public float ExpOnDeath;

        public static EnemyStats operator *(EnemyStats stat, SpecOption option)
        {
            EnemyStats result = new EnemyStats();
            result.Health = stat.Health * option.HealthUp;
            result.Attack = stat.Attack * option.AttackUp;
            result.Defence = stat.Defence * option.DefenceUp;
            result.MoveSpeed = stat.MoveSpeed * option.MoveSpeedUp;
            result.AttackSpeed = stat.AttackSpeed * option.AttackSpeedUp;
            result.Cooltime = stat.Cooltime * option.CooltimeDown;
            result.DetectRange = stat.DetectRange;
            result.AttackRange = stat.AttackRange;
            result.ItemDrop = stat.ItemDrop;
            result.GoldOnDeath = stat.GoldOnDeath;
            result.ExpOnDeath = stat.ExpOnDeath;
            return result;
        }
    }

}