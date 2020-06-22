using System;
using System.Diagnostics;
using System.Linq;
using OpenToolkit.Audio.OpenAL;

namespace AGS.Engine
{
    public class ALAudioBackend : IAudioBackend
    {
        private readonly int _playingState = (int)ALSourceState.Playing;
        private readonly ALContext _context;
        private readonly ALDevice _device;

        public ALAudioBackend()
        {
            try
            {
                (_context, _device) = loadContext();
                printSystem();
                IsValid = true;
            }
            catch (InvalidOperationException e1)
            {
                Debug.WriteLine("Failed to load audio context, audio will not be played.");
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
        }

        private (ALContext, ALDevice) loadContext()
        {
            var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
            checkALError();
            Debug.WriteLine($"Devices: {string.Join(", ", devices)}");

            string deviceName = devices.FirstOrDefault(d => d.Contains("OpenAL Soft")) ??
                ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);
            checkALError();

            var device = ALC.OpenDevice(deviceName);
            checkALError();

            var context = ALC.CreateContext(device, (int[])null);
            checkALError();

            ALC.MakeContextCurrent(context);
            checkALError();

            return (context, device);
        }

        private void printSystem()
        {
            ALC.GetInteger(_device, AlcGetInteger.MajorVersion, 1, out int alcMajorVersion);
            ALC.GetInteger(_device, AlcGetInteger.MinorVersion, 1, out int alcMinorVersion);
            string alcExts = ALC.GetString(_device, AlcGetString.Extensions);

            var attrs = ALC.GetContextAttributes(_device);
            Console.WriteLine($"Attributes: {attrs}");

            string exts = AL.Get(ALGetString.Extensions);
            string rend = AL.Get(ALGetString.Renderer);
            string vend = AL.Get(ALGetString.Vendor);
            string vers = AL.Get(ALGetString.Version);

            Debug.WriteLine($"Vendor: {vend}, \nVersion: {vers}, \nRenderer: {rend}, \nExtensions: {exts}, \nALC Version: {alcMajorVersion}.{alcMinorVersion}, \nALC Extensions: {alcExts}");
        }

        private static void checkALError()
        {
            ALError error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"AL Error: {AL.GetErrorString(error)}");
            }
        }


        public bool IsValid { get; }
        public int GenBuffer() => !IsValid ? 0 : AL.GenBuffer();
        public int GenSource() => !IsValid ? 0 : AL.GenSource();
        public void BufferData(int bufferId, SoundFormat format, byte[] buffer, int size, int freq)
        {
            if (!IsValid) return;
            AL.BufferData(bufferId, getFormat(format), buffer, size, freq);
        }
        public void BufferData(int bufferId, SoundFormat format, short[] buffer, int size, int freq)
        { 
            if (!IsValid) return;
            AL.BufferData(bufferId, getFormat(format), buffer, size, freq);
        }
        public void SourceStop(int sourceId) { if (!IsValid) return; AL.SourceStop(sourceId); }
        public void DeleteSource(int sourceId) { if (!IsValid) return;AL.DeleteSource(sourceId); }
        public void SourceSetBuffer(int sourceId, int bufferId) { if (!IsValid) return;AL.Source(sourceId, ALSourcei.Buffer, bufferId); }
        public void SourceSetLooping(int sourceId, bool looping) { if (!IsValid) return;AL.Source(sourceId, ALSourceb.Looping, looping); }
        public void SourceSetGain(int sourceId, float gain) { if (!IsValid) return;AL.Source(sourceId, ALSourcef.Gain, gain); }
        public void SourceSetPitch(int sourceId, float pitch) { if (!IsValid) return;AL.Source(sourceId, ALSourcef.Pitch, pitch); }
        public void SourceSetPosition(int sourceId, float x, float y, float z) { if (!IsValid) return;AL.Source(sourceId, ALSource3f.Position, x, y, z); }
        public void SourceSetSeek(int sourceId, float seek) { if (!IsValid) return;AL.Source(sourceId, ALSourcef.SecOffset, seek); }
        public float SourceGetSeek(int sourceId)
        {
            if (!IsValid) return -1f;
            float seek;
            AL.GetSource(sourceId, ALSourcef.SecOffset, out seek);
            return seek;
        }
        public void SourcePlay(int sourceId) { if (!IsValid) return; AL.SourcePlay(sourceId); }
        public void SourcePause(int sourceId) { if (!IsValid) return; AL.SourcePause(sourceId); }
        public void SourceRewind(int sourceId) { if (!IsValid) return; AL.SourceRewind(sourceId); }
        public bool SourceIsPlaying(int sourceId)
        {
            if (!IsValid) return false;
            int state;
            AL.GetSource(sourceId, ALGetSourcei.SourceState, out state);
            return state == _playingState;
        }
        public string GetError()
        {
            if (!IsValid) return null;
            ALError error = AL.GetError();
            if (error == ALError.NoError) return null;
            return error.ToString();
        }
        public void ListenerSetGain(float gain) { if (!IsValid) return; AL.Listener(ALListenerf.Gain, gain); }
        public void ListenerSetPosition(float x, float y, float z) { if (!IsValid) return; AL.Listener(ALListener3f.Position, x, y, z); }

        public void Dispose()
        {
            if (!IsValid) return;
            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(_context);
            ALC.CloseDevice(_device);
        }

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
