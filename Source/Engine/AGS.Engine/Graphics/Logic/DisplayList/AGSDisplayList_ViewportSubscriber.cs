using System;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSDisplayList
    {
        private struct ViewportSubscriber
        {
            private Action _onSomethingChanged;

            public ViewportSubscriber(IViewport viewport, Action onSomethingChanged)
            {
                _onSomethingChanged = onSomethingChanged;
                viewport.DisplayListSettings.PropertyChanged += onDisplayListPropertyChanged;
                viewport.DisplayListSettings.RestrictionList.PropertyChanged += onDisplayListPropertyChanged;
                viewport.DisplayListSettings.RestrictionList.RestrictionList.OnListChanged.Subscribe(onRestrictionListChanged);
                viewport.DisplayListSettings.DepthClipping.PropertyChanged += onDisplayListPropertyChanged;
            }

            private void onRestrictionListChanged(AGSHashSetChangedEventArgs<string> obj)
            {
                _onSomethingChanged();
            }

            private void onDisplayListPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                _onSomethingChanged();
            }
        }
    }
}
