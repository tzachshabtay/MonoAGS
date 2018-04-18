using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class ReduceWhenSpeechAudioRule : IAudioRule
    {
        private int _numOfPlayingSpeechSounds;
        private readonly IAudioSystem _audioSystem;
        private bool _enabled;

        private readonly VolumeModifier _modifier;

        public ReduceWhenSpeechAudioRule(IAudioSystem audioSystem, float volumeFactor = 0.3f)
        {
            _enabled = true;
            _audioSystem = audioSystem;
            _modifier = new VolumeModifier(volumeFactor);
        }

        public bool Enabled 
        {
            get => _enabled; 
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                if (value)
                {
                    if (_numOfPlayingSpeechSounds <= 0) return;
                    addModifierToAllSounds();
                }
                else removeModifierFromAllSounds();
            }
        }

        public void OnSoundCompleted(ISound sound)
        {
            if (sound.Tags.Contains(AGSSpeechLine.SpeechTag))
            {
                if (Interlocked.Decrement(ref _numOfPlayingSpeechSounds) == 0)
                {
                    removeModifierFromAllSounds();
                }
            }
        }

        public void OnSoundStarted(ISound sound)
        {
            if (sound.Tags.Contains(AGSSpeechLine.SpeechTag))
            {
                if (Interlocked.Increment(ref _numOfPlayingSpeechSounds) == 1)
                {
                    if (!Enabled) return;
                    addModifierToAllSounds();
                }
            }
            else if (Enabled && _numOfPlayingSpeechSounds > 0)
            {
                sound.SoundModifiers.Add(_modifier);
            }
        }

        private void addModifierToAllSounds()
        {
            foreach (var playingSound in _audioSystem.CurrentlyPlayingSounds)
            {
                if (playingSound.Tags.Contains(AGSSpeechLine.SpeechTag)) continue;
                playingSound.SoundModifiers.Add(_modifier);
            }
        }

        private void removeModifierFromAllSounds()
        {
            foreach (var playingSound in _audioSystem.CurrentlyPlayingSounds)
            {
                playingSound.SoundModifiers.Remove(_modifier);
            }
        }
    }
}