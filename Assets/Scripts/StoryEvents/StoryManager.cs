using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Singletons;

namespace StoryEvents
{
    public class StoryManager : PersistentSingleton<StoryManager>
    {
        [SerializeField]
        private List<StoryEvent> storyEvents;

        private void Start() 
        {
            LoadProgress();
        }

        public void UnlockEvent(string eventName)
        {
            var storyIndex = storyEvents.FindIndex(e => e.eventName == eventName);
            if (storyIndex >= 0)
            {
                var storyEvent = storyEvents[storyIndex];
                storyEvent.isUnlocked = true;
                Debug.Log($"Story Event {eventName} unlocked!");
                // todo: save the event

                // Trigger the event or handle any additional logic here
            }
            else
            {
                Debug.LogWarning($"Story Event {eventName} not found.");
            }
        }

        public bool IsEventUnlocked([NotNull] string eventName)
        {
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            var storyIndex = storyEvents.FindIndex(e => e.eventName == eventName);
            return storyIndex >= 0 && storyEvents[storyIndex].isUnlocked;
        }

        private void LoadProgress()
        {
            // todo: Load from a save file or database
        }

        private void OnApplicationQuit()
        {
            SaveProgress();
        }

        private void SaveProgress()
        {
            // todo: Save to a save file or database
        }

    }
}
