namespace AGS.API
{
    /// <summary>
    /// Allows adjusting the saturation of an entity/screen.
    /// If added as a component to an entity, the saturation property will control the entity's saturation.
    /// If created separately (and initialized with a null entity), this can be use to control the saturation of the entire screen.
    /// <example>
    /// <code language="lang-csharp">
    /// 
    /// //to adjust saturation for an object:
    /// myObj.AddComponent&lt;ISaturationEffectComponent&gt;().Saturation = 0.5f;
    /// 
    /// //to adjust saturation for the screen:
    /// var effect = new SaturationEffect(game.Factory.Shaders, game.Events);
    /// effect.Init(null);
    /// effect.Saturation = 0.5f;
    /// </code>
    /// </example>
    /// </summary>
    public interface ISaturationEffectComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the saturation for the entity/screen.
        /// The default is 1, which is the same color as the original image.
        /// A lower value than 1 will reduce saturation (0 being completely gray) and a greater value than 1 will increase saturation. 
        /// </summary>
        /// <value>The saturation.</value>
        float Saturation { get; set; }
    }
}
