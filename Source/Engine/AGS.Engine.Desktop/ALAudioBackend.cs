using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.OpenAL;

namespace AGS.Engine
{
    public class ALAudioBackend : IAudioBackend
    {
        private readonly int _playingState = (int)SourceState.Playing;
        private readonly AudioContext _context;
        private readonly AL _al;

        public ALAudioBackend()
        {
            try
            {
                _al = AL.GetApi();
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
        public int GenBuffer() => _context == null ? 0 : (int)_al.GenBuffer();
        public int GenSource() => _context == null ? 0 : (int)_al.GenSource();
        public unsafe void BufferData(int bufferId, SoundFormat format, byte[] buffer, int size, int freq)
        {
            if (_context == null) return;
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                _al.BufferData((uint)bufferId, getFormat(format), handle.AddrOfPinnedObject().ToPointer(), size, freq);
            }
            finally
            {
                handle.Free();
            }
        }
        public unsafe void BufferData(int bufferId, SoundFormat format, short[] buffer, int size, int freq)
        {
            if (_context == null)
                return;
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                _al.BufferData((uint)bufferId, getFormat(format), handle.AddrOfPinnedObject().ToPointer(), size, freq);
            }
            finally
            {
                handle.Free();
            }
        }
        public void SourceStop(int sourceId) { if (_context == null) return; _al.SourceStop((uint)sourceId); }
        public void DeleteSource(int sourceId) { if (_context == null) return;_al.DeleteSource((uint)sourceId); }
        public void SourceSetBuffer(int sourceId, int bufferId) { if (_context == null) return;_al.SetSourceProperty((uint)sourceId, SourceInteger.Buffer, bufferId); }
        public void SourceSetLooping(int sourceId, bool looping) { if (_context == null) return;_al.SetSourceProperty((uint)sourceId, SourceBoolean.Looping, looping); }
        public void SourceSetGain(int sourceId, float gain) { if (_context == null) return;_al.SetSourceProperty((uint)sourceId, SourceFloat.Gain, gain); }
        public void SourceSetPitch(int sourceId, float pitch) { if (_context == null) return;_al.SetSourceProperty((uint)sourceId, SourceFloat.Pitch, pitch); }
        public void SourceSetPosition(int sourceId, float x, float y, float z) { if (_context == null) return;_al.SetSourceProperty((uint)sourceId, SourceVector3.Position, x, y, z); }
        public void SourceSetSeek(int sourceId, float seek) { if (_context == null) return;_al.SetSourceProperty((uint)sourceId, SourceFloat.SecOffset, seek); }
        public float SourceGetSeek(int sourceId)
        {
            if (_context == null) return -1f;
            float seek;
            _al.GetSourceProperty((uint)sourceId, SourceFloat.SecOffset, out seek);
            return seek;
        }
        public void SourcePlay(int sourceId) { if (_context == null) return; _al.SourcePlay((uint)sourceId); }
        public void SourcePause(int sourceId) { if (_context == null) return; _al.SourcePause((uint)sourceId); }
        public void SourceRewind(int sourceId) { if (_context == null) return; _al.SourceRewind((uint)sourceId); }
        public bool SourceIsPlaying(int sourceId)
        {
            if (_context == null) return false;
            int state;
            _al.GetSourceProperty((uint)sourceId, GetSourceInteger.SourceState, out state);
            return state == _playingState;
        }
        public string GetError()
        {
            if (_context == null) return null;
            AudioError error = _al.GetError();
            if (error == AudioError.NoError) return null;
            return error.ToString();
        }
        public void ListenerSetGain(float gain) { if (_context == null) return; _al.SetListenerProperty(ListenerFloat.Gain, gain); }
        public void ListenerSetPosition(float x, float y, float z) { if (_context == null) return; _al.SetListenerProperty(ListenerVector3.Position, x, y, z); }

        public void Dispose() { if (_context == null) return; _context.Dispose(); }

        private BufferFormat getFormat(SoundFormat format)
        {
            switch (format)
            {
                case SoundFormat.Mono16: return BufferFormat.Mono16;
                case SoundFormat.Mono8: return BufferFormat.Mono8;
                case SoundFormat.Stereo16: return BufferFormat.Stereo16;
                case SoundFormat.Stereo8: return BufferFormat.Stereo8;
                default: throw new NotSupportedException(format.ToString());
            }
        }
    }
}
