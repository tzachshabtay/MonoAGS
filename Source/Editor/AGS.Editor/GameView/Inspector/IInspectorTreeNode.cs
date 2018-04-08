using System.Reflection;
using AGS.API;

namespace AGS.Editor
{
    public interface IInspectorTreeNode : ITreeStringNode
    {
        InspectorProperty Property { get; }

        IInspectorPropertyEditor Editor { get; }
    }
}
