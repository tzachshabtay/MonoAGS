using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IObject : IHasRoom, IAnimationContainer, IInTree<IObject>, IDisposable
	{
		string ID { get; }
		IInteractions Interactions { get; }
		ISquare BoundingBox { get; set; } //todo: find a way to remove the setter (only the engine should use the setter)
		IRenderLayer RenderLayer { get; set; }

		IPoint WalkPoint { get; set; }
		IPoint CenterPoint { get; }

		ICustomProperties Properties { get; }

		bool Enabled { get; set; }
		bool UnderlyingEnabled { get; }

		bool Visible { get; set; }
		bool UnderlyingVisible { get; }

		string Hotspot { get; set; }

		bool IgnoreViewport { get; set; }
		bool IgnoreScalingArea { get; set; }

		bool CollidesWith(float x, float y);
	}
}

