using System.Collections;
using UnityEngine;

namespace Scripts.Entities
{
    [CreateAssetMenu(menuName = "Entity/EntityData")]
    public class EntityData : ScriptableObject
    {
        public HealthData HealthData;
    }
}