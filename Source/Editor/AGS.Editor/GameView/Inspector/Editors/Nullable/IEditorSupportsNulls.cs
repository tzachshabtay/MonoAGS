using System;
namespace AGS.Editor
{
    public interface IEditorSupportsNulls : IInspectorPropertyEditor
    {
        void OnNullChanged(bool isNull);
    }
}