using System;
namespace AGS.Editor.Desktop
{
    public class DesktopEditorPlatform : IEditorPlatform
    {
        public DesktopEditorPlatform()
        {
            DotnetProject = new DotnetProject();
        }

        public IDotnetProject DotnetProject { get; private set; }
    }
}
