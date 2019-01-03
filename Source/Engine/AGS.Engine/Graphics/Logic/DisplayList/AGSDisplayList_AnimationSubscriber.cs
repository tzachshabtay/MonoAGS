using System;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSDisplayList
    {
        private class AnimationSubscriber
        {
            private IAnimationComponent _animationComponent;
            private IComponentBinding _animationComponentBinding;
            private IAnimation _lastAnimation;
            private ISprite _lastSprite;
            private float _lastX, _lastZ;
            private IEntity _obj;
            private Action _onSomethingChanged;

            private class BindingWrapper : IComponentBinding
            {
                private IComponentBinding _binding;
                private Action _unsubscribe;

                public BindingWrapper(IComponentBinding binding, Action unsubscribe)
                {
                    _binding = binding;
                    _unsubscribe = unsubscribe;
                }

                public void Unbind()
                {
                    _binding?.Unbind();
                    _unsubscribe();
                    _binding = null;
                    _unsubscribe = null;
                }
            }

            public AnimationSubscriber(IEntity obj, Action onSomethingChanged)
            {
                _animationComponentBinding = obj.Bind<IAnimationComponent>(c => _animationComponent = c, _ => _animationComponent = null);
                _obj = obj;
                _onSomethingChanged = onSomethingChanged;
                _lastAnimation = null;
                _lastSprite = null;
                _lastX = float.MinValue;
                _lastZ = float.MinValue;
            }

            public IComponentBinding Bind()
            {
                subscribeAnimation();
                return new BindingWrapper(bind<IAnimationComponent>(_obj, onObjAnimationPropertyChanged), unsubscribeAll);
            }

            private void unsubscribeAll()
            {
                unsubscribeLastAnimation();
                unsubscribeLastSprite();
                _animationComponentBinding?.Unbind();
            }

            private void onObjAnimationPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IAnimationComponent.Animation)) return;
                subscribeAnimation();
            }

            private void subscribeAnimation()
            {
                unsubscribeLastAnimation();
                _lastAnimation = _animationComponent?.Animation;
                var state = _animationComponent?.Animation?.State;
                if (state != null)
                {
                    state.PropertyChanged += onAnimationStatePropertyChanged;
                }
                subscribeSprite();
            }

            private void unsubscribeLastAnimation()
            {
                var state = _lastAnimation?.State;
                if (state != null)
                {
                    state.PropertyChanged -= onAnimationStatePropertyChanged;
                }
                _lastAnimation = null;
            }

            private void onAnimationStatePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IAnimationState.CurrentFrame)) return;
                subscribeSprite();
            }

            private void onSpritePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(ISprite.Z)) onSpriteZChange();
                else if (args.PropertyName == nameof(ISprite.X)) onSpriteXChange();
            }

            private void onSpriteZChange()
            {
                var sprite = _animationComponent?.Animation?.Sprite;
                if (sprite == null) return;
                if (MathUtils.FloatEquals(_lastZ, sprite.Z)) return;
                _lastZ = sprite.Z;
                _onSomethingChanged();
            }

            private void onSpriteXChange()
            {
                var sprite = _animationComponent?.Animation?.Sprite;
                if (sprite == null) return;
                if (MathUtils.FloatEquals(_lastX, sprite.X)) return;
                _lastX = sprite.X;
                _onSomethingChanged();
            }

            private void subscribeSprite()
            {
                unsubscribeLastSprite();
                var newSprite = _animationComponent?.Animation?.Sprite;
                if (newSprite != null) newSprite.PropertyChanged += onSpritePropertyChanged;
                _lastSprite = newSprite;
                onSpriteZChange();
                onSpriteXChange();
            }

            private void unsubscribeLastSprite()
            {
                var lastSprite = _lastSprite;
                if (lastSprite != null) lastSprite.PropertyChanged -= onSpritePropertyChanged;
                _lastSprite = null;
            }
        }
    }
}
