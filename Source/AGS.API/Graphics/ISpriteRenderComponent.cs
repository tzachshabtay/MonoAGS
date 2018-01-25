using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Interface of a single sprite source.
    /// </summary>
    public interface ISpriteProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a sprite to work with.
        /// </summary>
        /// <value>The sprite.</value>
        ISprite Sprite { get; }
    }

    /// <summary>
    /// A component that renders an entity using 2D sprites.
    /// A sprite and other properties may be set either by user or by other components.
    /// </summary>
    public interface ISpriteRenderComponent : IComponent
    {
        /// <summary>
        /// Gets sprite to render.
        /// </summary>
        /// <value>The sprite.</value>
        ISprite CurrentSprite { get; }

        /// <summary>
        /// Gets or sets sprite provider.
        /// </summary>
        /// <value>The sprite provider implementation.</value>
        ISpriteProvider SpriteProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pivot (the pivot point for position/rotate/scale) will 
        /// be drawn on the screen as a cross. This can be used for debugging the game.
        /// </summary>
        /// <value><c>true</c> if debug draw pivot; otherwise, <c>false</c>.</value>
		bool DebugDrawPivot { get; set; }

        /// <summary>
        /// Gets or sets a border that will (optionally) surround the sprite.
        /// </summary>
        /// <value>The border.</value>
		IBorderStyle Border { get; set; }
    }
}
