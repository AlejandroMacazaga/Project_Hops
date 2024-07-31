using Entities;

namespace Enemies
{
    public class EnemyController : EntityController
    {
        private new BasicEnemyData data => (BasicEnemyData)base.data;
    }
}