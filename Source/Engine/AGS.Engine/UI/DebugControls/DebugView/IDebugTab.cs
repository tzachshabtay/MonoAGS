using System.Threading.Tasks;

namespace AGS.Engine
{
    public interface IDebugTab
    {
        Task Show();
        void Hide();
    }
}
