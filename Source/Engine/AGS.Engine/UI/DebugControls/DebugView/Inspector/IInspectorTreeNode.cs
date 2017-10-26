using System.Reflection;
using AGS.API;

namespace AGS.Engine
{
    public interface IInspectorTreeNode : ITreeStringNode
    {
        InspectorProperty Property { get; }

        IInspectorPropertyEditor Editor { get; }
    }
}
