using System.Collections.Concurrent;
using AGS.API;

namespace AGS.Engine
{
    public class AGSFocusedUI : IFocusedUI, IModalWindows
    {
        public AGSFocusedUI()
        {
            ModalWindows = new ConcurrentStack<IEntity>();
            CannotLoseFocus = new AGSConcurrentHashSet<string>();
        }

        public ITextBoxComponent FocusedTextBox { get; set; }

        public IEntity FocusedWindow
        {
            get
            {
                IEntity result;
                if (ModalWindows.TryPeek(out result)) return result;
                return null;
            }
        }

        public ConcurrentStack<IEntity> ModalWindows { get; }

        public IConcurrentHashSet<string> CannotLoseFocus { get; }
    }
}
