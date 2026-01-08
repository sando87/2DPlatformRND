
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
        public float GoldOnDeath;
        public float ExpOnDeath;

        // ----- += 연산 메서드 -----
        public void Add(EnemyStats other)
        {
            Health += other.Health;
            Attack += other.Attack;
            Defence += other.Defence;
            MoveSpeed += other.MoveSpeed;
            AttackSpeed += other.AttackSpeed;
            Cooltime += other.Cooltime;
            DetectRange += other.DetectRange;
            AttackRange += other.AttackRange;
            ItemDrop += other.ItemDrop;
            GoldOnDeath += other.GoldOnDeath;
            ExpOnDeath += other.ExpOnDeath;

        }

        // ----- -= 연산 메서드 -----
        public void Subtract(EnemyStats other)
        {
            Health -= other.Health;
            Attack -= other.Attack;
            Defence -= other.Defence;
            MoveSpeed -= other.MoveSpeed;
            AttackSpeed -= other.AttackSpeed;
            Cooltime -= other.Cooltime;
            DetectRange -= other.DetectRange;
            AttackRange -= other.AttackRange;
            ItemDrop -= other.ItemDrop;
            GoldOnDeath -= other.GoldOnDeath;
            ExpOnDeath -= other.ExpOnDeath;
        }
    }

}