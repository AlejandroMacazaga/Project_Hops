using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Utils.Timers
{
    internal static class TimerBootstrapper
    {
        private static PlayerLoopSystem _timerSystem;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            
            if (!InsertTimerManager<Update>(ref currentPlayerLoop, 0))
            {
                Debug.LogError("Failed to insert TimerManager into PlayerLoop");
                return;
            }
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
#endif
            static void OnPlayModeState(PlayModeStateChange state)
            {
                if (state != PlayModeStateChange.ExitingPlayMode) return;
                PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                RemoveTimerManager<Update>(ref currentPlayerLoop);
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                TimerManager.Clear();
            }
        }
        
        // Remove TimerManager from PlayerLoop

        static void RemoveTimerManager<T>(ref PlayerLoopSystem loop)
        {
            PlayerLoopUtils.RemoveSystem<T>(ref loop, in _timerSystem);
        }
        static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index)
        {
            var timerSystem = new PlayerLoopSystem()
            {
                type = typeof(TimerManager),
                updateDelegate = TimerManager.Update,
                subSystemList = null
            };

            return PlayerLoopUtils.InsertSystem<T>(ref loop, in timerSystem, index);
        }
        
        
    }
}