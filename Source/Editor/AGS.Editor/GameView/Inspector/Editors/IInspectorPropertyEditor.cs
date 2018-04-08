using System;
using AGS.API;

namespace AGS.Editor
{
    public interface IInspectorPropertyEditor
    {
        void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property);

        void RefreshUI();
    }
}
