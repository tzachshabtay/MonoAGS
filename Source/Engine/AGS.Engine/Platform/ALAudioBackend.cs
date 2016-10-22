using System;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace AGS.Engine
{
    public class ALAudioBackend : IAudioBackend
    {
        private readonly int _playingState = (int)ALSourceState.Playing;
        private AudioContext _context;

        public ALAudioBackend()
        {
            _context = new AudioContext();
        }

        public int GenBuffer() { return AL.GenBuffer(); }
        public int GenSource() { return AL.GenSource(); }
        public void BufferData(int bufferId, SoundFormat format, byte[] buffer, int size, int freq)
        {
            AL.BufferData(bufferId, getFormat(format), buffer, size, freq);
        }
        public void BufferData(int bufferId, SoundFormat format, short[] buffer, int size, int freq)
        { 
            AL.BufferData(bufferId, getFormat(format), buffer, size, freq);
        }
        public void SourceStop(int sourceId) { AL.SourceStop(sourceId); }
        public void DeleteSource(int sourceId) { AL.DeleteSource(sourceId); }
        public void SourceSetBuffer(int sourceId, int bufferId) { AL.Source(sourceId, ALSourcei.Buffer, bufferId); }
        public void SourceSetLooping(int sourceId, bool looping) { AL.Source(sourceId, ALSourceb.Looping, looping); }
        public void SourceSetGain(int sourceId, float gain) { AL.Source(sourceId, ALSourcef.Gain, gain); }
        public void SourceSetPitch(int sourceId, float pitch) { AL.Source(sourceId, ALSourcef.Pitch, pitch); }
        public void SourceSetPosition(int sourceId, float x, float y, float z) { AL.Source(sourceId, ALSource3f.Position, x, y, z); }
        public void SourceSetSeek(int sourceId, float seek) { AL.Source(sourceId, ALSourcef.SecOffset, seek); }
        public float SourceGetSeek(int sourceId)
        {
            float seek;
            AL.GetSource(sourceId, ALSourcef.SecOffset, out seek);
            return seek;
        }
        public void SourcePlay(int sourceId) { AL.SourcePlay(sourceId); }
        public void SourcePause(int sourceId) { AL.SourcePause(sourceId); }
        public void SourceRewind(int sourceId) { AL.SourceRewind(sourceId); }
        public bool SourceIsPlaying(int sourceId)
        {
            int state;
            AL.GetSource(sourceId, ALGetSourcei.SourceState, out state);
            return state == _playingState;
        }
        public string GetError()
        {
            ALError error = AL.GetError();
            if (error == ALError.NoError) return null;
            return error.ToString();
        }
        public void ListenerSetGain(float gain) { AL.Listener(ALListenerf.Gain, gain); }
        public void ListenerSetPosition(float x, float y, float z) { AL.Listener(ALListener3f.Position, x, y, z); }

        public void Dispose() { _context.Dispose(); }

        private ALFormat getFormat(SoundFormat format)
        {
            switch (format)
            {
                case SoundFormat.Mono16: return ALFormat.Mono16;
                case SoundFormat.Mono8: return ALFormat.Mono8;
                case SoundFormat.Stereo16: return ALFormat.Stereo16;
                case SoundFormat.Stereo8: return ALFormat.Stereo8;
                default: throw new NotSupportedException(format.ToString());
            }
        }
    }
}
