using System;

namespace AGS.API
{
	public interface IObject : IEntity, IHasRoom, IAnimationContainer, IInTree<IObject>, ICollider, 
		IVisibleComponent, IEnabledComponent, ICustomProperties, IDrawableInfo, IHotspotComponent
	{
	}
}

