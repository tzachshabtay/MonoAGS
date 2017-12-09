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
        /// <code>
        /// cHero.Tint = Colors.Red; //will give our here a red tint.
        /// </code>
        /// </example>
        /// </summary>
        /// <value>The tint.</value>
        Color Tint { get; set; }

        /// <summary>
        /// Gets or sets the anchor. The anchor for an image is the pivot point from which 
        /// the position, scale and rotation are determined. For example, rotating an image
        /// from it's center point will rotate it in place, while rotating it from it's bottom-left
        /// point will rotate the entire image around the bottom-left. I might help to think of an anchor
        /// as the point on an image on which you place your finger on before rotating.
        ///
        /// The units of the anchor point is in relation for the image size, where (0f,0f) is the bottom-left
        /// corner, and (1f,1f) is the top-right corner. The default is (0.5f, 0f), which means the bottom-center
        /// point of the image.
        /// </summary>
        /// <example>
        /// <code>
        /// image.Anchor = new PointF(0.5f, 0.5f); //placing the anchor point in the middle of the image
        /// </code>
        /// </example>
        /// <value>The anchor.</value>
        PointF Anchor { get; set; }

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
    [RequiredComponent(typeof(IAnimationContainer))]
    [RequiredComponent(typeof(IScaleComponent))]
    public interface IImageComponent : IHasImage, IComponent
    {
    }
}
