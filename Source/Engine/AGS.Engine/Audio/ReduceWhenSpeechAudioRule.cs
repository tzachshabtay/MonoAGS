using System;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class ReduceWhenSpeechAudioRule : IAudioRule
    {
        private int _numOfPlayingSpeechSounds;
        private readonly IAudioSystem _audioSystem;
        private bool _enabled;
        private readonly float _targetVolumeFactor, _fadeOutTimeInSeconds;
        private readonly Func<float, float> _easing;

        private VolumeModifier _modifier;
        private Tween _runningTween;

        public ReduceWhenSpeechAudioRule(IAudioSystem audioSystem, float volumeFactor = 0.2f, float fadeOutTimeInSeconds = 0.8f, Func<float, float> easing = null)
        {
            _targetVolumeFactor = volumeFactor;
            _fadeOutTimeInSeconds = fadeOutTimeInSeconds;
            _easing = easing;
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
            var currentTween = _runningTween;
            if (currentTween != null)
            {
                currentTween.Stop(TweenCompletion.Stay);
            }
            foreach (var playingSound in _audioSystem.CurrentlyPlayingSounds)
            {
                if (playingSound.Tags.Contains(AGSSpeechLine.SpeechTag)) continue;
                playingSound.SoundModifiers.Add(_modifier);
            }
            _modifier.VolumeFactor = 1f;
            _runningTween = Tween.Run(1f, _targetVolumeFactor, vol => _modifier.VolumeFactor = vol, _fadeOutTimeInSeconds, _easing);
        }

        private async void removeModifierFromAllSounds()
        {
            var currentTween = _runningTween;
            if (currentTween != null)
            {
                currentTween.Stop(TweenCompletion.Stay);
            }
            _runningTween = Tween.Run(_modifier.VolumeFactor, 1f, vol => _modifier.VolumeFactor = vol, _fadeOutTimeInSeconds, _easing);
            await _runningTween.Task;
            if (_runningTween.Task.IsCompleted)
            {
                foreach (var playingSound in _audioSystem.CurrentlyPlayingSounds)
                {
                    playingSound.SoundModifiers.Remove(_modifier);
                }
            }
        }
    }
}