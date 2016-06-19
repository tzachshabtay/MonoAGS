namespace AGS.API
{
    public interface ILabel<TControl> : ITextComponent, IUIControl<TControl> where TControl : IUIControl<TControl>
	{
	}

	public interface ILabel : ILabel<ILabel>
	{}
}