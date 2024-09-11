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

    [CreateAssetMenu(menuName = "ZoneData")]
    public class GroupOfZones : ScriptableObject
    {
        public int[] sceneIds;
    }
}