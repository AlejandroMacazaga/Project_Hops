using System;
using UnityEngine.Serialization;
using Utils.EventBus;
using Eflatun.SceneReference;
namespace Utils.MultiSceneSystem
{
    [Serializable]
    public struct LoadSceneGroupEvent : IEvent
    {
        public SceneReference[] toLoad;
    }
}