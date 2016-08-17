using System;
namespace AGS.API
{
	[RequiredComponent(typeof(IWalkBehavior))]
	[RequiredComponent(typeof(IHasRoom))]
	[RequiredComponent(typeof(ITransformComponent))]
	public interface IFollowBehavior : IComponent
	{
		void Follow(IObject obj, IFollowSettings settings = null); 
	}
}

