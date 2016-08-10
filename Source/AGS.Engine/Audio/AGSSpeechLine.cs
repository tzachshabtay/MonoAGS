using AGS.API;

namespace AGS.Engine
{
    public class AGSSpeechLine : ISpeechLine
    {
        public AGSSpeechLine(IAudioClip audioClip, string text)
        {
            AudioClip = audioClip;
            Text = text;
        }

        public IAudioClip AudioClip { get; private set; }

        public string Text { get; private set; }
    }
}

