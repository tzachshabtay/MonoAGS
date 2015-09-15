using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IObject : ISprite, IInTree<IObject>
	{
		IRoom Room { get; set; }
		IAnimation Animation { get; }
		IInteractions Interactions { get; }
		ISquare BoundingBox { get; set; } //todo: find a way to remove the setter (only the engine should use the setter)
		IRenderLayer RenderLayer { get; set; }

		bool Visible { get; set; }
		bool Enabled { get; set; }
		string Hotspot { get; set; }

		bool IgnoreViewport { get; set; }

		bool DebugDrawAnchor { get; set; }

		IBorderStyle Border { get; set; }

		void StartAnimation(IAnimation animation);
		AnimationCompletedEventArgs Animate(IAnimation animation);
		Task<AnimationCompletedEventArgs> AnimateAsync(IAnimation animation);

		void ChangeRoom(IRoom room, float? x = null, float? y = null);
	}
}

