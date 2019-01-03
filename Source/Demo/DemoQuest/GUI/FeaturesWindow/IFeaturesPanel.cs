using System.Threading.Tasks;

namespace DemoGame
{
    public interface IFeaturesPanel
    {
        void Show();
        Task Close();
    }
}
