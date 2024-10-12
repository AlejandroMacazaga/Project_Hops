using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Helpers
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsCache = new();
        
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (WaitForSecondsCache.TryGetValue(seconds, out var waitForSeconds)) return waitForSeconds;
            
            waitForSeconds = new WaitForSeconds(seconds);
            WaitForSecondsCache.Add(seconds, waitForSeconds);

            return WaitForSecondsCache[seconds];
        }
    }
}