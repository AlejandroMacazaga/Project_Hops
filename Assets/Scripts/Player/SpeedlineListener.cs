using KBCore.Refs;
using Player.Events;
using UnityEngine;
using Utils.EventBus;

namespace Player
{
    public class SpeedlineListener : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private ParticleSystem speedLines;

        void Start()
        {
            speedLines.Stop();
            EventBus<PlayerIsGoingFast>.Register(new EventBinding<PlayerIsGoingFast>(HandleSpeedLine));
        }

        void HandleSpeedLine(PlayerIsGoingFast e) 
        {
            if (e.IsGoingFast)
            {
                speedLines.Play();
            }
            else
            {
                Invoke(nameof(StopSpeedLine), 0.1f);
            }
        }

        void StopSpeedLine()
        {
            speedLines.Stop();
        }
    
    }
}
