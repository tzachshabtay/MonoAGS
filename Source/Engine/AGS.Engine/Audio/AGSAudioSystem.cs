using System.Collections.ObjectModel;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSAudioSystem: IAudioSystem
    {
        private readonly AGSConcurrentHashSet<ISound> _playingSounds;

        public AGSAudioSystem(IAudioSettings settings)
        {
            _playingSounds = new AGSConcurrentHashSet<ISound>();
            AudioClips = new AGSBindingList<IAudioClip>(30);
            AudioClips.OnListChanged.Subscribe(onAudioClipsChanged);
            AudioRules = new AGSBindingList<IAudioRule>(5);
            Settings = settings;
        }

        public IAudioSettings Settings { get; }

        public ReadOnlyCollection<ISound> CurrentlyPlayingSounds => _playingSounds.ToList().AsReadOnly();

        public IAGSBindingList<IAudioClip> AudioClips { get; }

        public IAGSBindingList<IAudioRule> AudioRules { get; }

        private void onAudioClipsChanged(AGSListChangedEventArgs<IAudioClip> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var clip in args.Items)
                {
                    clip.Item.OnSoundStarted.Subscribe(onSoundStarted);
                    clip.Item.OnSoundCompleted.Subscribe(onSoundCompleted);
                }
            }
            else
            {
                foreach (var clip in args.Items)
                {
                    clip.Item.OnSoundStarted.Unsubscribe(onSoundStarted);
                    clip.Item.OnSoundCompleted.Unsubscribe(onSoundCompleted);
                }
            }
        }

        private void onSoundStarted(ISound sound)
        {
            _playingSounds.Add(sound);
            foreach (var rule in AudioRules)
            {
                rule.OnSoundStarted(sound);
            }
        }

        private void onSoundCompleted(ISound sound)
        {
            _playingSounds.Remove(sound);
            foreach (var rule in AudioRules)
            {
                rule.OnSoundCompleted(sound);
            }
        }
    }
}