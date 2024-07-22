using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    [CreateAssetMenu(menuName = "Entity/EntityData")]
    public class EntityData : ScriptableObject
    {
        public HealthData HealthData;
    }
}