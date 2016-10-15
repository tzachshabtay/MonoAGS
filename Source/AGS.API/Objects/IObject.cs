namespace AGS.API
{
	public interface IObject : IEntity, IHasRoom, IAnimationContainer, IInObjectTree, ICollider, 
		IVisibleComponent, IEnabledComponent, ICustomPropertiesComponent, IDrawableInfo, IHotspotComponent, 
        IShaderComponent, ITranslateComponent, IImageComponent, IScaleComponent, IRotateComponent, 
        IPixelPerfectComponent, IHasModelMatrix//, ISprite
	{
	}
}

