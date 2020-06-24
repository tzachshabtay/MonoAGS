using AGS.API;

namespace AGS.Engine
{
    public class AGSModalWindowComponent : AGSComponent, IModalWindowComponent
    {
        private IModalWindows _modalWindows;
        private IFocusedUI _focusedUi;

        public AGSModalWindowComponent(IModalWindows modalWindows, IFocusedUI focusedUI)
        {
            _modalWindows = modalWindows;
            _focusedUi = focusedUI;
        }

		public override void Dispose()
		{
            LoseFocus();
            base.Dispose();
		}

        public bool HasFocus => _focusedUi.FocusedWindow == Entity;

        public void GrabFocus()
        {
            if (HasFocus) return;
            _modalWindows.ModalWindows.Push(Entity);
        }

        public void LoseFocus()
        {
            if (!HasFocus) return;
            _modalWindows.ModalWindows.TryPop(out IEntity _);
        }
    }
}
