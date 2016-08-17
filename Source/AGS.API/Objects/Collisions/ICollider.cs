using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IDrawableInfo))]
	[RequiredComponent(typeof(IAnimationContainer))]
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IPixelPerfectComponent))]
	public interface ICollider : IComponent
	{
		ISquare BoundingBox { get; set; } //todo: find a way to remove the setter (only the engine should use the setter)
		PointF? CenterPoint { get; }
		bool CollidesWith(float x, float y);
	}
}

