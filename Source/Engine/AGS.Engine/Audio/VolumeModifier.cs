using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
    public class VolumeModifier : ISoundModifier
    {
        private float _volumeFactor;

        public VolumeModifier(float volumeFactor)
        {
            OnChange = new AGSEvent();
            _volumeFactor = volumeFactor;
        }

        public float VolumeFactor
        {
            get => _volumeFactor;
            set
            {
                if (MathUtils.FloatEquals(_volumeFactor, value)) return;
                _volumeFactor = value;
                OnChange.Invoke();
            }
        }

        public IBlockingEvent OnChange { get; }

        public float GetPanning(float panning) => panning;

        public float GetPitch(float pitch) => pitch;

        public float GetVolume(float volume) => volume * VolumeFactor;
    }
}
