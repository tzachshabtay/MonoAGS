namespace AGS.API
{
    public interface ISlider<TControl> : ISliderComponent, IUIControl<TControl> where TControl : IUIControl<TControl>
	{}

	public interface ISlider : ISlider<ISlider>
	{}
}

