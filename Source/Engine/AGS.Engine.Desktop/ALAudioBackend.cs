using System;
using System.Diagnostics;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace AGS.Engine
{
    public class ALAudioBackend : IAudioBackend
    {
        private readonly int _playingState = (int)ALSourceState.Playing;
        private readonly AudioContext _context;

        public ALAudioBackend()
        {
            try
            {
                _context = new AudioContext();
                IsValid = true;
            }
            catch (AudioContextException e1)
            {
                Debug.WriteLine("Failed to create audio context, audio will not be played.");
                Debug.WriteLine(e1.ToString());
            }
            catch (DllNotFoundException e2)
            {
                Debug.WriteLine("Failed to find OpenAL Soft dll, audio will not be played.");
                Debug.WriteLine(e2.ToString());
            }
            catch (TypeInitializationException e3)
            {
                Debug.WriteLine("Failed to load OpenAL Soft dll (this might because the dll is missing, or corrupt, or another reason), audio will not be played.");
                Debug.WriteLine(e3.ToString());
            }
            catch (AudioDeviceException e4)
            {
                Debug.WriteLine("Failed to load audio device, audio will not be played.");
                Debug.WriteLine(e4.ToString());
            }
        }

        public bool IsValid { get; }
        public int GenBuffer() => _context == null ? 0 : AL.GenBuffer();
        public int GenSource() => _context == null ? 0 : AL.GenSource();
        public void BufferData(int bufferId, SoundFormat format, byte[] buffer, int size, int freq)
        {
            if (_context == null) return;
            AL.BufferData(bufferId, getFormat(format), buffer, size, freq);
        }
        public void BufferData(int bufferId, SoundFormat format, short[] buffer, int size, int freq)
        { 
            if (_context == null) return;
            AL.BufferData(bufferId, getFormat(format), buffer, size, freq);
        }
        public void SourceStop(int sourceId) { if (_context == null) return; AL.SourceStop(sourceId); }
        public void DeleteSource(int sourceId) { if (_context == null) return;AL.DeleteSource(sourceId); }
        public void SourceSetBuffer(int sourceId, int bufferId) { if (_context == null) return;AL.Source(sourceId, ALSourcei.Buffer, bufferId); }
        public void SourceSetLooping(int sourceId, bool looping) { if (_context == null) return;AL.Source(sourceId, ALSourceb.Looping, looping); }
        public void SourceSetGain(int sourceId, float gain) { if (_context == null) return;AL.Source(sourceId, ALSourcef.Gain, gain); }
        public void SourceSetPitch(int sourceId, float pitch) { if (_context == null) return;AL.Source(sourceId, ALSourcef.Pitch, pitch); }
        public void SourceSetPosition(int sourceId, float x, float y, float z) { if (_context == null) return;AL.Source(sourceId, ALSource3f.Position, x, y, z); }
        public void SourceSetSeek(int sourceId, float seek) { if (_context == null) return;AL.Source(sourceId, ALSourcef.SecOffset, seek); }
        public float SourceGetSeek(int sourceId)
        {
            if (_context == null) return -1f;
            float seek;
            AL.GetSource(sourceId, ALSourcef.SecOffset, out seek);
            return seek;
        }
        public void SourcePlay(int sourceId) { if (_context == null) return; AL.SourcePlay(sourceId); }
        public void SourcePause(int sourceId) { if (_context == null) return; AL.SourcePause(sourceId); }
        public void SourceRewind(int sourceId) { if (_context == null) return; AL.SourceRewind(sourceId); }
        public bool SourceIsPlaying(int sourceId)
        {
            if (_context == null) return false;
            int state;
            AL.GetSource(sourceId, ALGetSourcei.SourceState, out state);
            return state == _playingState;
        }
        public string GetError()
        {
            if (_context == null) return null;
            ALError error = AL.GetError();
            if (error == ALError.NoError) return null;
            return error.ToString();
        }
        public void ListenerSetGain(float gain) { if (_context == null) return; AL.Listener(ALListenerf.Gain, gain); }
        public void ListenerSetPosition(float x, float y, float z) { if (_context == null) return; AL.Listener(ALListener3f.Position, x, y, z); }

        public void Dispose() { if (_context == null) return; _context.Dispose(); }

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
