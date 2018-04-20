using AGS.API;

namespace AGS.Engine
{
    public class PanningModifier : ISoundModifier
    {
        private float _panningOffset;

        public PanningModifier(float panningOffset)
        {
            OnChange = new AGSEvent();
            _panningOffset = panningOffset;
        }

        public float PanningOffset
        {
            get => _panningOffset;
            set
            {
                if (MathUtils.FloatEquals(_panningOffset, value)) return;
                _panningOffset = value;
                OnChange.Invoke();
            }
        }

        public IBlockingEvent OnChange { get; }

        public float GetPanning(float panning) => MathUtils.Clamp(panning + _panningOffset, -1f, 1f);

        public float GetPitch(float pitch) => pitch;

        public float GetVolume(float volume) => volume;
    }
}