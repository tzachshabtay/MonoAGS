using System;

namespace AGS.API
{
	public interface IObject : IHasRoom, IAnimationContainer, IInTree<IObject>, ICollider, IDisposable
	{
		string ID { get; }
		IInteractions Interactions { get; }

		IRenderLayer RenderLayer { get; set; }

		IPoint WalkPoint { get; set; }

		ICustomProperties Properties { get; }

		bool Enabled { get; set; }
		bool UnderlyingEnabled { get; }

		bool Visible { get; set; }
		bool UnderlyingVisible { get; }

		string Hotspot { get; set; }

		bool IgnoreViewport { get; set; }
		bool IgnoreScalingArea { get; set; }
	}
}

