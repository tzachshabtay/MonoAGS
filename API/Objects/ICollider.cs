using System;

namespace AGS.API
{
	public interface ICollider
	{
		ISquare BoundingBox { get; set; } //todo: find a way to remove the setter (only the engine should use the setter)
		IPoint CenterPoint { get; }
		bool CollidesWith(float x, float y);
	}
}

