using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class EditorShouldBlockEngineInput : IShouldBlockInput
    {
        private readonly IShouldBlockInput _defaultBlocker;

        public EditorShouldBlockEngineInput(IGameState state)
        {
            _defaultBlocker = new AGSShouldBlockInput(state);
        }

        public bool BlockEngine { get; set; }

        public bool ShouldBlockInput()
        {
            return BlockEngine || _defaultBlocker.ShouldBlockInput();
        }
    }
}