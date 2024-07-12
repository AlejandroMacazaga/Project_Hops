using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EventBusUtils
{
    public static IReadOnlyList<Type> EventTypes {  get; set; }
    public static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR
    public static PlayModeStateChange PlayModeState { get; set; }

    [InitializeOnLoadMethod]
    public static void InitializeEditor()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        PlayModeState = state;
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            ClearAllBuses();
        }
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        EventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
        EventBusTypes = InitializeAllBuses();
    }

    static List<Type> InitializeAllBuses()
    {
        var eventBusTypes = new List<Type>();

        var typedef = typeof(EventBus<>);
        foreach (var type in EventTypes)
        {
            var busType = typedef.MakeGenericType(type);
            eventBusTypes.Add(busType);
            Debug.Log($"Initialized EventBus<{type.Name}>");
        }
        return eventBusTypes;
    }

    public static void ClearAllBuses()
    {
        Debug.Log("Clearing all the buses...");
        for (int i = 0; i < EventBusTypes.Count; i++)
        {
            var busType = EventBusTypes[i];
            var clearMethod = busType.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            clearMethod.Invoke(null, null);
        }
    }

}