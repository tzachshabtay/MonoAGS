using System;

namespace API
{
	public interface ILabel<TControl> : IUIControl<TControl> where TControl : IUIControl<TControl>
	{
		ITextConfig TextConfig { get; set; }
		string Text { get; set; }
	}

	public interface ILabel : ILabel<ILabel>
	{}
}

