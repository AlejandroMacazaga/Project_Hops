using Eflatun.SceneReference;
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
    
    public class GroupOfZones : ScriptableObject
    {
        public int[] sceneIds;
    }
    
    public class LevelZones : ScriptableObject
    {
        public GroupOfZones[] zonesInLevel;
    }

    [CreateAssetMenu(menuName = "Levels/Scene Group")]
    public class SceneGroup : ScriptableObject
    {
        public SceneReference[] scenes;
    }
    
}