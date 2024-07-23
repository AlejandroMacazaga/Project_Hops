namespace StoryEvents
{
    [System.Serializable]
    public struct StoryEvent
    {
        public string eventName;
        public bool isUnlocked;
        public string unlockConditionDescription; // For debugging or design purposes
    }
}