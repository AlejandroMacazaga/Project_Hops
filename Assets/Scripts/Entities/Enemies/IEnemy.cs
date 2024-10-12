namespace Entities.Enemies
{
    public interface IEnemy
    {
        public float GetStat(EnemyStat stat);
    }

    public enum EnemyStat
    {
        AttackDamage,
        AttackSpeed
    }
}