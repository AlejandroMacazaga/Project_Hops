using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(menuName = "Entity/EntityData")]
    public class EntityData : ScriptableObject
    {
        public HealthData healthData;
    }
}