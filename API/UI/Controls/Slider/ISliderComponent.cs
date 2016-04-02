using System;

namespace AGS.API
{
	[RequiredComponent(typeof(ICollider))]
	[RequiredComponent(typeof(IDrawableInfo))]
	[RequiredComponent(typeof(IInObjectTree))]
	[RequiredComponent(typeof(IVisibleComponent))]
	[RequiredComponent(typeof(IEnabledComponent))]
	public interface ISliderComponent : IComponent
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
}

