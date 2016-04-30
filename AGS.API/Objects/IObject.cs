using System;

namespace AGS.API
{
	public interface IObject : IEntity, IHasRoom, IAnimationContainer, IInObjectTree, ICollider, 
		IVisibleComponent, IEnabledComponent, ICustomProperties, IDrawableInfo, IHotspotComponent, IShaderComponent
	{
	}
}

