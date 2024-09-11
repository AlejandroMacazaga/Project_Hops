using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utils.MultiSceneSystem
{
    [CreateAssetMenu(menuName = "ZoneData")]
    public class ZoneData : ScriptableObject
    {
        public int sceneBuildIndex;
        public Vector3 zonePosition;
    }
}