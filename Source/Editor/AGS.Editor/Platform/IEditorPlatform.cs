using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public interface IEditorPlatform
    {
        IDotnetProject DotnetProject { get; }

        void SetResolverForGame(Resolver gameResolver, Resolver editorResolver);

        void SetHostedGameWindow(Rectangle windowSize);

        ISerialization GetSerialization(IGame game);
    }
}