using AGS.API;

namespace AGS.Engine
{
    public class AGSModalWindowComponent : AGSComponent, IModalWindowComponent
    {
        private IModalWindows _modalWindows;
        private IFocusedUI _focusedUi;
        private IEntity _entity;

        public AGSModalWindowComponent(IModalWindows modalWindows, IFocusedUI focusedUI, IGameEvents gameEvents)
        {
            _modalWindows = modalWindows;
            _focusedUi = focusedUI;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
        }

		public override void Dispose()
		{
            base.Dispose();
            LoseFocus();
            _entity = null;
		}

		public bool HasFocus => _focusedUi.FocusedWindow == _entity;

        public void GrabFocus()
        {
            if (HasFocus) return;
            _modalWindows.ModalWindows.Push(_entity);
        }

        public void LoseFocus()
        {
            if (!HasFocus) return;
            IEntity entity;
            _modalWindows.ModalWindows.TryPop(out entity);
        }
    }
}
