using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public static class HoverEffect
    {
		public static void Add(IObject obj, Color idleTint, Color hoverTint)
		{
			obj.Tint = idleTint;
			var uiEvents = obj.AddComponent<IUIEvents>();
			uiEvents.MouseEnter.Subscribe(_ => obj.Tint = hoverTint);
			uiEvents.MouseLeave.Subscribe(_ => obj.Tint = idleTint);
		}

        public static void Add(params (IObject obj, ButtonAnimation idle, ButtonAnimation hover)[] effects)
        {
            Add(null, null, effects);
        }

        public static void Add(Action onEnter, Action onLeave, params (IObject obj, ButtonAnimation idle, ButtonAnimation hover)[] effects)
        {
            int numHovered = 0;
            foreach (var (obj, idle, _) in effects)
            {
                idle?.StartAnimation(obj);
                var uiEvents = obj.AddComponent<IUIEvents>();
                uiEvents.MouseEnter.Subscribe(_ =>
                {
                    numHovered++;
                    foreach (var (o, _, hover) in effects)
                    {
                        hover?.StartAnimation(o);
                    }
                    onEnter?.Invoke();
                });
                uiEvents.MouseLeave.Subscribe(async _ =>
                {
                    numHovered--;
                    Trace.Assert(numHovered >= 0);
                    if (numHovered == 0)
                    {
                        await Task.Delay(10);
                        if (numHovered == 0)
                        {
                            foreach (var (o, i, _) in effects)
                            {
                                i?.StartAnimation(o);
                            }
                            onLeave?.Invoke();
                        }
                    }
                });
            }
        }
    }
}
