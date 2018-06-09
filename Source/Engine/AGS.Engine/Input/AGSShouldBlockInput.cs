using AGS.API;

namespace AGS.Engine
{
    public class AGSShouldBlockInput : IShouldBlockInput
    {
        private readonly IGameState _state;

        public AGSShouldBlockInput(IGameState state)
        {
            _state = state;
        }

        public bool ShouldBlockInput()
        {
            if (_state.Room == null || _state.DuringRoomTransition) return true;
            return false;
        }
    }
}