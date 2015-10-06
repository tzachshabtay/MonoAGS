using System;

namespace AGS.API
{
	public interface ILabel<TControl> : IUIControl<TControl> where TControl : IUIControl<TControl>
	{
		ITextConfig TextConfig { get; set; }
		string Text { get; set; }

		float TextHeight  { get; }
		float TextWidth { get; }
	}

	public interface ILabel : ILabel<ILabel>
	{}
}

