using System;
using System.Collections.Generic;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSDisplayList
    {
        private class EntitySubscriber
        {
            private Action<int> _onLayerChanged;
            private int _layer;
            private List<IComponentBinding> _bindings;
            private IInObjectTreeComponent _tree;

            public EntitySubscriber(IEntity entity, Action<int> onLayerChanged)
            {
                _onLayerChanged = onLayerChanged;
                entity.Bind<IDrawableInfoComponent>(c => _layer = c.RenderLayer?.Z ?? 0, _ => _layer = 0);

                var vBinding = bind<IVisibleComponent>(entity, onObjVisibleChanged);
                var tBinding = bind<ITranslateComponent>(entity, onObjTranslatePropertyChanged);
                var dBinding = bind<IDrawableInfoComponent>(entity, onObjDrawablePropertyChanged);
                var eBinding = bind<IAreaComponent>(entity, onAreaPropertyChanged);
                var bBinding = bind<IWalkBehindArea>(entity, onWalkBehindPropertyChanged);

                AnimationSubscriber animSubscriber = new AnimationSubscriber(entity, onSomethingChanged);
                var aBinding = animSubscriber.Bind();

                var trBinding = entity.Bind<IInObjectTreeComponent>(c => { _tree = c; c.TreeNode.OnParentChanged.Subscribe(onSomethingChanged); onSomethingChanged(); }, c => { _tree = null; c.TreeNode.OnParentChanged.Unsubscribe(onSomethingChanged); onSomethingChanged(); });

                _bindings = new List<IComponentBinding> { vBinding, tBinding, dBinding, aBinding, trBinding, eBinding, bBinding };
            }

            public void Unsubscribe()
            {
                _tree?.TreeNode.OnParentChanged.Unsubscribe(onSomethingChanged);
                foreach (var binding in _bindings) binding?.Unbind();
            }

            private void onObjVisibleChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IVisibleComponent.Visible)) return;
                onSomethingChanged();
            }

            private void onObjTranslatePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(ITranslateComponent.Z)) return;
                onSomethingChanged();
            }

            private void onObjDrawablePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IDrawableInfoComponent.RenderLayer)) return;
                onSomethingChanged();
            }

            private void onAreaPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                onSomethingChanged();
            }

            private void onWalkBehindPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                onSomethingChanged();
            }

            private void onSomethingChanged()
            {
                _onLayerChanged(_layer);
            }
        }
    }
}
