namespace AGS.API
{
	public interface IObject : IEntity, IHasRoom, IAnimationContainer, IInObjectTree, ICollider, 
		IVisibleComponent, IEnabledComponent, ICustomProperties, IDrawableInfo, IHotspotComponent, 
        IShaderComponent, ITransformComponent, IImageComponent, IScaleComponent, IRotateComponent, 
        IPixelPerfectComponent, IHasModelMatrix//, ISprite
	{
	}
}

