using System;
namespace AGS.Editor
{
    public interface IEditorPlatform
    {
        IDotnetProject DotnetProject { get; }
    }
}
