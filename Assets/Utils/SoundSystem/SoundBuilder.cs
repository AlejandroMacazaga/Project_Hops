using UnityEngine;

namespace Utils.SoundSystem
{
    public class SoundBuilder 
    {
        private readonly SoundManager _soundManager;
        private SoundData _soundData;
        private Vector3 _position = Vector3.zero;
        public bool RandomPitch = false;
        
        public SoundBuilder(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        
        public SoundBuilder WithSoundData(SoundData soundData)
        {
            _soundData = soundData;
            return this;
        }
        
        public SoundBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public void Play()
        {
            if (!_soundManager.CanPlaySound(_soundData)) return;

            var soundEmitter = _soundManager.Get();
            soundEmitter.Initialize(_soundData);
            soundEmitter.transform.position = _position;
            if (RandomPitch) soundEmitter.WithRandomPitch();
            
            if (_soundData.frequentSound)
            {
                _soundManager.FrequentSounds.Enqueue(soundEmitter);
            }
            soundEmitter.Play();
        }
    }
}
