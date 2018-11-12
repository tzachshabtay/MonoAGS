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
	}
}
