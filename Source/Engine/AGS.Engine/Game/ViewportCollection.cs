using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class ViewportCollection
    {
        private readonly IGameState _state;

        public ViewportCollection(IGameState state)
        {
            _state = state;
            subscribeViewport(state.Viewport);
            state.SecondaryViewports.OnListChanged.Subscribe(onViewportsChanged);
            refresh();
        }

        public List<IViewport> SortedViewports { get; private set; }

        private void onViewportsChanged(AGSListChangedEventArgs<IViewport> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var viewport in args.Items)
                {
                    subscribeViewport(viewport.Item);
                }
            }
            else
            {
                foreach (var viewport in args.Items)
                {
                    unsubscribeViewport(viewport.Item);
                }
            }
            refresh();
        }

        private void subscribeViewport(IViewport viewport)
        {
            viewport.PropertyChanged += onViewportPropertyChanged;
        }

        private void unsubscribeViewport(IViewport viewport)
        {
            viewport.PropertyChanged -= onViewportPropertyChanged;
        }

        private void onViewportPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (nameof(IViewport.Z) != args.PropertyName) return;
            refresh();
        }

        private void refresh()
        {
            List<IViewport> viewports = new List<IViewport>(_state.SecondaryViewports.Count + 1);
            viewports.Add(_state.Viewport);
            viewports.AddRange(_state.SecondaryViewports);
            SortedViewports = viewports.OrderBy(viewport => viewport.Z).ToList();
        }
    }
}
