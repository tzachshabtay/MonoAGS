using AGS.API;

namespace AGS.Engine
{
	public interface IAGSHitTest : IHitTest
    {
        void Refresh(MousePosition mousePosition);
    }
}