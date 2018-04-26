using AGS.API;

namespace AGS.Engine
{
    public class AGSSpeechLine : ISpeechLine
    {
        public AGSSpeechLine(IAudioClip audioClip, string text)
        {
            AudioClip = audioClip;
            audioClip?.Tags.Add(SpeechTag);
            Text = text;
        }

        public const string SpeechTag = "Speech";

        public IAudioClip AudioClip { get; private set; }

        public string Text { get; private set; }
    }
}

