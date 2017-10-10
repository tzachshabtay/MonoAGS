using AGS.API;

namespace AGS.Engine
{
    public interface IInspectorTreeNode : ITreeStringNode
    {
        string Value { get; }
    }
}
