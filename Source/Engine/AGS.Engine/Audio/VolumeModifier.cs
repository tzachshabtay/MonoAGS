using System;
using AGS.API;

namespace AGS.Engine
{
    public class VolumeModifier : ISoundModifier
    {
        public VolumeModifier(float volumeFactor)
        {
            OnChange = new AGSEvent();
            VolumeFactor = volumeFactor;
        }

        public float VolumeFactor { get; private set; }

        public IEvent OnChange { get; }

        public float GetPanning(float panning) => panning;

        public float GetPitch(float pitch) => pitch;

        public float GetVolume(float volume) => volume * VolumeFactor;
    }
}