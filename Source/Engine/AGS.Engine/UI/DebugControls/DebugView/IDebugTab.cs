using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public interface IDebugTab
    {
        IPanel Panel { get; }
        Task Show();
        void Hide();
    }
}
