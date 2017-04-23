using System;

namespace AGS.Engine
{
    public enum SoundFormat
    { 
        Mono8,
        Mono16,
        Stereo8,
        Stereo16,
    }

    public interface IAudioBackend : IDisposable
    {
        /// <summary>
        /// Is the audio backend working (are audio clips expected to play properly or is there some issue)?
        /// </summary>
        bool IsValid { get; }
        int GenBuffer();
        int GenSource();
        void BufferData(int bufferId, SoundFormat format, byte[] buffer, int size, int freq);
        void BufferData(int bufferId, SoundFormat format, short[] buffer, int size, int freq);
        void SourceStop(int sourceId);
        void DeleteSource(int sourceId);
        void SourceSetBuffer(int sourceId, int bufferId);
        void SourceSetLooping(int sourceId, bool looping);
        void SourceSetGain(int sourceId, float gain);
        void SourceSetPitch(int sourceId, float pitch);
        void SourceSetPosition(int sourceId, float x, float y, float z);
        void SourceSetSeek(int sourceId, float seek);
        float SourceGetSeek(int sourceId);
        void SourcePlay(int sourceId);
        void SourcePause(int sourceId);
        void SourceRewind(int sourceId);
        bool SourceIsPlaying(int sourceId);
        string GetError();
        void ListenerSetGain(float gain);
        void ListenerSetPosition(float x, float y, float z);
    }
}
