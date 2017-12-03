using System;
using AGS.API;

namespace AGS.Engine
{
    public interface IInspectorPropertyEditor
    {
        void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property);

        void RefreshUI();
    }
}
