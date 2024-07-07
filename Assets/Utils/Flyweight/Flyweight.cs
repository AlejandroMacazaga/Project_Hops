using System.Collections;
using UnityEngine;
namespace Utils.Flyweight
{
    public class Flyweight : MonoBehaviour
    {
        public FlyweightSettings settings;
        
        
        protected IEnumerator DespawnAfterDelay()
        {
            yield return Helpers.GetWaitForSeconds(settings.lifeTime);
            FlyweightManager.Instance.ReturnToPool(this);
        }
    }
}