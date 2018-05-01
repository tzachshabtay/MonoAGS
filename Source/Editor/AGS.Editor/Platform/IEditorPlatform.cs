using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public interface IEditorPlatform
    {
        IDotnetProject DotnetProject { get; }

        void SetResolverForGame(Resolver resolver);

        void SetHostedGameWindow(Rectangle windowSize);
    }
}