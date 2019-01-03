using System;
using System.Collections.Generic;
using System.Reflection;
using AGS.API;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AGS.Engine
{
    public static class Extensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, Func<TValue> getValue)
        {
            if (!map.TryGetValue(key, out var value))
            {
                value = getValue();
                map[key] = value;
            }
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, Func<TKey, TValue> getValue)
        {
            if (!map.TryGetValue(key, out var value))
            {
                value = getValue(key);
                map[key] = value;
            }
            return value;
        }

        public static TEntity Remember<TEntity>(this TEntity entity, IGame game,
            Action<TEntity> resetEntity) where TEntity : class, IEntity
        {
            resetEntity(entity);
            game.Events.OnSavedGameLoad.Subscribe(() =>
            {
                entity = game.Find<TEntity>(entity.ID);
                resetEntity(entity);
            });
            return entity;
        }

        public static IEnumerable<Type> GetInterfaces(this Type someType)
        {
            var t = someType;
            while (t != null)
            {
                var ti = t.GetTypeInfo();
                foreach (var m in ti.ImplementedInterfaces)
                    yield return m;
                t = ti.BaseType;
            }
        }

        public static float AlignX(this ITextConfig config, float width, SizeF baseSize)
        {
            switch (config.Alignment)
            {
                case Alignment.TopLeft:
                case Alignment.MiddleLeft:
                case Alignment.BottomLeft:
                    return config.PaddingLeft;
                case Alignment.TopCenter:
                case Alignment.MiddleCenter:
                case Alignment.BottomCenter:
                    float left = config.PaddingLeft;
                    float right = baseSize.Width - config.PaddingRight;
                    return (left + right) / 2f - width / 2f;
                default:
                    return baseSize.Width - width - config.PaddingRight;
            }
        }

        public static float AlignY(this ITextConfig config, float bitmapHeight, float height, SizeF baseSize)
        {
            switch (config.Alignment)
            {
                case Alignment.TopLeft:
                case Alignment.TopCenter:
                case Alignment.TopRight:
                    return bitmapHeight - baseSize.Height + config.PaddingTop;
                case Alignment.MiddleLeft:
                case Alignment.MiddleCenter:
                case Alignment.MiddleRight:
                    return bitmapHeight - baseSize.Height / 2f - height / 2f;
                default:
                    return bitmapHeight - height - config.PaddingBottom;
            }
        }

        public static Vector3 ToVector3(this PointF point)
        {
            return new Vector3(point.X, point.Y, 0f);
        }

        public static Vector2 ToVector2(this PointF point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static GLColor ToGLColor(this Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            float a = color.A / 255f;
            return new GLColor(r, g, b, a);
        }

        public static SizeF GetTextSize(this ITextConfig config, string text, SizeF labelSize)
        {
            config = AGSTextConfig.ScaleConfig(config, GLText.TextResolutionFactorX);
            labelSize = labelSize.Scale(GLText.TextResolutionFactorX, GLText.TextResolutionFactorY);
            float scaleBackX = 1f / GLText.TextResolutionFactorX;
            float scaleBackY = 1f / GLText.TextResolutionFactorY;

            switch (config.AutoFit)
            {
                case AutoFit.TextShouldFitLabel:
                    var textSize = config.Font.MeasureString(text, config.Alignment);
                    return new SizeF(Math.Min(textSize.Width, labelSize.Width), Math.Min(textSize.Height, labelSize.Height)).Scale(scaleBackX, scaleBackY);
                case AutoFit.TextShouldWrapAndLabelShouldFitHeight:
                    return config.Font.MeasureString(text, config.Alignment, (int)labelSize.Width).Scale(scaleBackX, scaleBackY);
                default:
                    return config.Font.MeasureString(text, config.Alignment).Scale(scaleBackX, scaleBackY);
            }
        }

        public static void StartAnimation(this ButtonAnimation button, IAnimationComponent animationComponent,
                                          ITextComponent textComponent, IImageComponent imageComponent, IBorderComponent borderComponent)
        {
            if (button == null) return;

            var animation = button.Animation;
            if (animation != null && animation.Frames.Count > 0) animationComponent?.StartAnimation(animation);

            var border = button.Border;
            if (border != null && borderComponent != null) borderComponent.Border = border;

            var textConfig = button.TextConfig;
            if (textConfig != null && textComponent != null) textComponent.TextConfig = textConfig;

            if (imageComponent != null)
            {
                var image = button.Image;
                if (image != null) imageComponent.Image = image;

                var tint = button.Tint;
                if (tint != null) imageComponent.Tint = tint.Value;
            }
        }

        public static void RunOnTree<TItem>(this ITreeNode<TItem> node, int level, Action<TItem, int> action)
		 where TItem : class, IInTree<TItem>
        {
            action(node.Node, level);
            foreach (var child in node.Children)
            {
                child.TreeNode.RunOnTree(level + 1, action);
            }
        }

        public static void DestroyWithChildren(this IObject obj, IGameState state = null)
        {
            if (obj == null) return;
            state = state ?? AGSGame.Game.State;
            if (obj.Room != null)
                obj.Room.Objects.Remove(obj);
            else state.UI.Remove(obj);
            for (int i = obj.TreeNode.ChildrenCount - 1; i >= 0; i--)
                obj.TreeNode.Children[i].DestroyWithChildren(state);
            obj.Dispose();
        }

        private class Disposer : IDisposable
        {
            private Action _dispose;

            public Disposer(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose?.Invoke();
                _dispose = null;
            }
        }

        /// <summary>
        /// Gives the ability to subscribe to property change event (i.e to react whenever a specific property changes its value).
        /// </summary>
        /// <example>
        /// Let's say, for example, that you want that you're working in a two-buttons control scheme (left click to interact, right click to examine)
        /// but you don't have an inventory window, but rather you want to change the cursor whenever the active inventory item for your player character changes.
        /// You can do:
        /// <code language="lang-csharp">
        /// player.Inventory.OnPropertyChanged(nameof(IInventory.ActiveItem), () => twoButtonsScheme.SetInventoryCursor());
        /// </code>
        /// </example>
        /// <returns>A disposable object is returned which allows you to unsubscribe from the event (by calling its Dispose method).</returns>
        /// <param name="notifier">The object which contains the property which you want to track.</param>
        /// <param name="propertyName">The property name you which to track.</param>
        /// <param name="callback">The action to be performed whenever the property changes.</param>
        public static IDisposable OnPropertyChanged(this INotifyPropertyChanged notifier, string propertyName, Action callback)
        {
            PropertyChangedEventHandler handler = (object sender, PropertyChangedEventArgs e) => 
            { 
                if (e.PropertyName == propertyName) callback(); 
            };
            notifier.PropertyChanged += handler;
            var disposer = new Disposer(() => notifier.PropertyChanged -= handler);
            return disposer;
        }
        
        public static bool IsBetween(this float x, float min, float max) => x >= min && x <= max;
	}
}
