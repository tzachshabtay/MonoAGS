using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IObject : IAnimationContainer, IInTree<IObject>
	{
		IRoom Room { get; }
		IInteractions Interactions { get; }
		ISquare BoundingBox { get; set; } //todo: find a way to remove the setter (only the engine should use the setter)
		IRenderLayer RenderLayer { get; set; }

		IPoint WalkPoint { get; set; }
		IPoint CenterPoint { get; }

		ICustomProperties Properties { get; }

		bool Enabled { get; set; }
		string Hotspot { get; set; }

		bool IgnoreViewport { get; set; }
		bool IgnoreScalingArea { get; set; }

		void ChangeRoom(IRoom room, float? x = null, float? y = null);
		bool CollidesWith(float x, float y);
	}
}

