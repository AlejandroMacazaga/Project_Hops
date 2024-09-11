using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utils.MultiSceneSystem
{
    public class ZoneData : ScriptableObject
    {
        public int sceneBuildIndex;
        public Vector3 zonePosition;
    }

    [CreateAssetMenu(menuName = "Levels/GroupOfZones")]
    public class GroupOfZones : ScriptableObject
    {
        public int[] sceneIds;
    }

    [CreateAssetMenu(menuName = "Levels/Level")]
    public class LevelZones : ScriptableObject
    {
        public GroupOfZones[] zonesInLevel;
    }
}