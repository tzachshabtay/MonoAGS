using System;

namespace AGS.API
{
	public interface ISlider<TControl> : IUIControl<TControl> where TControl : IUIControl<TControl>
	{
		IObject Graphics { get; set; }
		IObject HandleGraphics { get; set; }
		ILabel Label { get; set; }

		float MinValue { get; set; }
		float MaxValue { get; set; }
		float Value { get; set; }
		bool IsHorizontal { get; set; }

		IEvent<SliderValueEventArgs> OnValueChanged { get; }
	}

	public interface ISlider : ISlider<ISlider>
	{}
}

