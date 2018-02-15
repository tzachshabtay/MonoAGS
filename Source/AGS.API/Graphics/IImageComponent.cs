using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// A container for an image.
    /// </summary>
    /// <seealso cref="AGS.API.IComponent" />
    public interface IHasImage : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the opacity of the object. 0 for fully transparent, 255 for fully opaque.
        /// </summary>
        /// <value>The opacity.</value>
        byte Opacity { get; set; }

        /// <summary>
        /// Gets or sets the tinting color for the object.
        /// <example>
        /// <code language="lang-csharp">
        /// cHero.Tint = Colors.Red; //will give our here a red tint.
        /// </code>
        /// </example>
        /// </summary>
        /// <value>The tint.</value>
        Color Tint { get; set; }

        /// <summary>
        /// Gets or sets the pivot point from which the position, scale and rotation are determined. 
        /// For example, rotating an image from it's center point will rotate it in place, 
        /// while rotating it from it's bottom-left point will rotate the entire image around the bottom-left. 
        /// It might help to think of the pivot as the point on an image on which you place your finger on before rotating.
        ///
        /// The units of the pivot point is in relation for the image size, where (0f,0f) is the bottom-left
        /// corner, and (1f,1f) is the top-right corner. The default is (0.5f, 0f), which means the bottom-center
        /// point of the image.
        /// </summary>
        /// <example>
        /// <code language="lang-csharp">
        /// image.Pivot = new PointF(0.5f, 0.5f); //placing the pivot point in the middle of the image
        /// </code>
        /// </example>
        /// <value>The pivot.</value>
        PointF Pivot { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        IImage Image { get; set; }

        /// <summary>
        /// Gets or sets a custom renderer for the image.
        /// This is null by default which uses the engine's default renderer, but you can implement a custom renderer
        /// if you desire.
        /// </summary>
        /// <value>The custom renderer.</value>
        IImageRenderer CustomRenderer { get; set; }
    }

    /// <summary>
    /// A component which allows setting an image to the entity.
    /// </summary>
    [RequiredComponent(typeof(IScaleComponent))]
    public interface IImageComponent : IHasImage, IComponent
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
