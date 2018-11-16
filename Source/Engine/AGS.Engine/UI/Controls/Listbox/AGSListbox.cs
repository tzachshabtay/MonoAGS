using AGS.API;

namespace AGS.Engine
{
	public class AGSListbox : IListbox
    {
        public AGSListbox(IPanel scrollingPanel, IPanel contentsPanel, IListboxComponent listboxComponent)
        {
            ScrollingPanel = scrollingPanel;
            ContentsPanel = contentsPanel;
            ListboxComponent = listboxComponent;
        }

        public IPanel ScrollingPanel { get; }

        public IPanel ContentsPanel { get; }

        public IListboxComponent ListboxComponent { get; }
    }
}