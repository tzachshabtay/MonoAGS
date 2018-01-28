using System.Threading.Tasks;
using AGS.API;

namespace DemoGame
{
    public interface IFeaturesPanel
    {
        void Show();
        Task Close();
    }
}
