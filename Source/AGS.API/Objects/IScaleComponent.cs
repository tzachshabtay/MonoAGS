using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Allows scaling (changing the size of) entities/sprites.
    /// </summary>
    public interface IScale: INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the height (in pixels).
        /// </summary>
        /// <value>The height.</value>
        float Height { get; }

        /// <summary>
        /// Gets the width (in pixels).
        /// </summary>
        /// <value>The width.</value>
        float Width { get; }

        /// <summary>
        /// Gets or sets the current horizontal scale (1 by default- meaning no scale).
        /// The scaling is calculated based on the original size, meaning that ScaleX * <see cref="BaseSize"/>.Width = <see cref="Width"/>.  
        /// </summary>
        /// <value>The scale x.</value>
        float ScaleX { get; set; }

        /// <summary>
        /// Gets or sets the current vertical scale (1 by default- meaning no scale).
        /// The scaling is calculated based on the original size, meaning that ScaleY * <see cref="BaseSize"/>.Height = <see cref="Height"/>.  
        /// </summary>
        /// <value>The scale y.</value>
        float ScaleY { get; set; }

        /// <summary>
        /// Scales the size by a factor. The factor is calculated on top of the original size,
        /// not the current size. (1,1) is the default, meaning no scale.
        /// </summary>
        /// <example>
        /// <code>
        /// sprite.ResetScale(10f,10f);
        /// sprite.Scale = new PointF(2f, 1f);
        /// Debug.WriteLine("Size of sprite is ({0},{1})", sprite.Width, sprite.Height); //prints "Size of sprite is (20,10)"
        /// sprite.Scale = new PointF(3f, 4f);
        /// Debug.WriteLine("Size of sprite is ({0},{1})", sprite.Width, sprite.Height); //prints "Size of sprite is (30,40)"
        /// </code>
        /// </example>
        PointF Scale { get; set; }

        /// <summary>
        /// Gets or sets the base size (the "original" size), on top of which the scale is calculated.
        /// </summary>
        /// <value>The size of the base.</value>
        SizeF BaseSize { get; set; }

        /// <summary>
        /// Resets the scale to (1,1), i.e no scaling.
        /// </summary>
        void ResetScale();

        /// <summary>
        /// Resets the base size (the "original" size, <see cref="BaseSize"/>) to the specified scale,
        /// and then resets the scale to (1,1), i.e no scaling.
        /// </summary>
        /// <param name="initialWidth">Initial width.</param>
        /// <param name="initialHeight">Initial height.</param>
        void ResetScale(float initialWidth, float initialHeight);

        /// <summary>
        /// Scales the size to a specific size. This will change <see cref="ScaleX"/> and <see cref="ScaleY"/> to fit.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <example>
        /// <code>
        /// sprite.ResetScale(10f,10f);
        /// sprite.ScaleTo(20f,30f);
        /// Debug.WriteLine("Sprite is scaled by ({0},{1})", sprite.ScaleX, sprite.ScaleY); //prints "Sprite is scaled by (2,3)"
        /// </code>
        /// </example>
        void ScaleTo(float width, float height);

        /// <summary>
        /// Flips the scale horizontally (this is done by negating <see cref="ScaleX"/> and <see cref="IHasImage.Pivot"/>'s X).
        /// </summary>
        void FlipHorizontally();

        /// <summary>
        /// Flips the scale vertucally (this is done by negating <see cref="ScaleY"/> and <see cref="IHasImage.Pivot"/>'s Y).  
        /// </summary>
        void FlipVertically();
    }

    /// <summary>
    /// Allows scaling (changing the size of) an entity.
    /// </summary>
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(IAnimationComponent), false)]
    public interface IScaleComponent : IScale, IComponent
    {
    }
}
