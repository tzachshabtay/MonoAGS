namespace AGS.API
{
    /// <summary>
    /// The model matrix is used for rendering and hit-testing the object.
    /// </summary>
    public struct ModelMatrices
    {
        /// <summary>
        /// The object's matrix calculated in the object's resolution (used for rendering).
        /// </summary>
        public Matrix4 InObjResolutionMatrix;
        /// <summary>
        /// The object's matrix calculated in the game's resoution (used for hit-testing).
        /// </summary>
        public Matrix4 InVirtualResolutionMatrix;
    }

    /// <summary>
    /// A component to calculate the matrix used to render/hit-test the object.
    /// The matrix includes transormations, rotations and scale.
    /// </summary>
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(ITranslateComponent))]
    [RequiredComponent(typeof(IAnimationContainer))]
    [RequiredComponent(typeof(IRotateComponent))]
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(IHasRoom))]
    [RequiredComponent(typeof(IDrawableInfo))]
    [RequiredComponent(typeof(IInObjectTree))]
    public interface IModelMatrixComponent : IComponent
    {
        /// <summary>
        /// Gets the model matrices.
        /// </summary>
        /// <returns>The model matrices.</returns>
        ModelMatrices GetModelMatrices();

        /// <summary>
        /// An event that fires whenever the matrix changes.
        /// </summary>
        /// <value>The on matrix changed.</value>
        IEvent<AGSEventArgs> OnMatrixChanged { get; }
    }
}
