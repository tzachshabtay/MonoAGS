namespace AGS.API
{
    public interface IPanel<TControl> : IUIControl<TControl> where TControl : IUIControl<TControl>
	{}

	public interface IPanel : IPanel<IPanel>
	{}
}

