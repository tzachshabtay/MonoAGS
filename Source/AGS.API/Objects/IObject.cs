namespace AGS.API
{
    /// <summary>
    /// An object is an entity with pre-set components (like location, scale, rotation, animation, etc) which is useful
    /// to depict all adventure game objects. Both characters and UI controls are also objects (with additional components).
    /// </summary>
	public interface IObject : IEntity, IHasRoomComponent, IAnimationComponent, IInObjectTreeComponent, IColliderComponent, 
		IVisibleComponent, IEnabledComponent, ICustomPropertiesComponent, IDrawableInfoComponent, 
        IShaderComponent, ITranslateComponent, IImageComponent, IScaleComponent, IRotateComponent, 
        IPixelPerfectComponent, IHasModelMatrix, IModelMatrixComponent, IBoundingBoxComponent, IWorldPositionComponent
	{
	}
}

