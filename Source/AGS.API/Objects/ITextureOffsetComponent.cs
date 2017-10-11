using System;
namespace AGS.API
{
	/// <summary>
	/// Allows changing the starting offset from which the texture will be rendered.
	/// 
	/// This can be useful, for example, when dealing with a tiled texture 
    /// (i.e when the <see cref="ITextureConfig"/> has either wrapX or wrapY as <see cref="TextureWrap.Repeat"/> or <see cref="TextureWrap.MirroredRepeat"/>)
    /// when you want to animate the tile.
	/// </summary>
	public interface ITextureOffsetComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the texture starting offset from which it will be rendered.
        /// The units are in relation to the texture's size, where (0,0) is the top-left of the texture,
        /// and (1,1) is the bottom-right of the texture (note that if the value is under 0 or above 1, the behavior
        /// of the texture is determined based on wrapping configurations in <see cref="ITextureConfig"/>.
        /// </summary>
        /// <value>The texture offset.</value>
        PointF TextureOffset { get; set; }

        /// <summary>
        /// An event which fires when the texture offset changes.
        /// </summary>
        /// <value>The event.</value>
        IEvent OnTextureOffsetChanged { get; }
    }
}
