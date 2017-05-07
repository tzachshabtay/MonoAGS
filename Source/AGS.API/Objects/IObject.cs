namespace AGS.API
{
    /// <summary>
    /// An object is an entity with pre-set components (like location, scale, rotation, animation, etc) which is useful
    /// to depict all adventure game objects. Both characters and UI controls are also objects (with additional components).
    /// </summary>
	public interface IObject : IEntity, IHasRoom, IAnimationContainer, IInObjectTree, ICollider, 
		IVisibleComponent, IEnabledComponent, ICustomPropertiesComponent, IDrawableInfo, IHotspotComponent, 
        IShaderComponent, ITranslateComponent, IImageComponent, IScaleComponent, IRotateComponent, 
        IPixelPerfectComponent, IHasModelMatrix, IModelMatrixComponent
	{
	}
}

