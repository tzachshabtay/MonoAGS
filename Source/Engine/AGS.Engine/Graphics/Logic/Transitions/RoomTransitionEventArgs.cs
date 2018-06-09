using System;
using AGS.API;

namespace AGS.Engine
{
    public class RoomTransitionEventArgs
    {
        public RoomTransitionEventArgs(IRoom from, IRoom to, Action afterTransitionFadeOut)
        {
            From = from;
            To = to;
            AfterTransitionFadeOut = afterTransitionFadeOut;
        }

        public IRoom From { get; }
        public IRoom To { get; }
        public Action AfterTransitionFadeOut { get; }
    }
}