using AGS.API;

namespace AGS.Engine
{
    public class AGSShouldBlockInput : IShouldBlockInput
    {
        private IGameState _state;

        public AGSShouldBlockInput(IGameState state)
        {
            _state = state;
        }

        public bool ShouldBlockInput()
        {
            if (_state.Room == null || _state.RoomTransitions.State != RoomTransitionState.NotInTransition) return true;
            return false;
        }
    }
}
