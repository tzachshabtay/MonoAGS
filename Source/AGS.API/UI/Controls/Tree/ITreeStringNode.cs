using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// A string tree node, which will be shown in a <see cref="ITreeViewComponent"/> .
    /// </summary>
    public interface ITreeStringNode : IStringItem, IInTree<ITreeStringNode>, INotifyPropertyChanged
    {
    }
}
