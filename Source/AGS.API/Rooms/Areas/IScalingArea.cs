namespace AGS.API
{
    /// <summary>
    /// Scaling axis on which the scaling takes place.
    /// </summary>
    public enum ScalingAxis
    {
        /// <summary>
        /// Scale based on the horizontal X axis.
        /// </summary>
        X,
        /// <summary>
        /// Scale based on the vertical Y axis.
        /// </summary>
        Y,
    }

    /// <summary>
    /// This component adds the ability for an area to automatically scale each object/character within it
    /// (and also 'scale' the volume of sounds coming from that object).
    /// The most common use is for faking perspective, so that when a character walks to the top of the screen
    /// (closer to the horizon), he will appear smaller and make quieter sounds.
    /// For that common use case, you'll want to scale on the Y axis (which is why Y axis is the default),
    /// and scale both the object x &amp;&amp; y (to keep the aspect ration in tact).
    /// </summary>
    [RequiredComponent(typeof(IArea))]
    public interface IScalingArea : IComponent
	{
        /// <summary>
        /// Gets or sets the minimum scaling for this area.
        /// if the scaling axis is Y (the default), this is the scaling that will be performed at the very top of the area.
        /// if the scaling axis is X, this is the scaling that will be performed at the very left of the area.
        /// Note that if for some reason you want the opposite behavior for either of the axis, you can make MaxScaling smaller than MinScaling.
        /// </summary>
        /// <value>The minimum scaling.</value>
		float MinScaling { get; set; }

        /// <summary>
        /// Gets or sets the maximum scaling for this area.
        /// if the scaling axis is Y (the default), this is the scaling that will be performed at the very bottom of the area.
        /// if the scaling axis is X, this is the scaling that will be performed at the very right of the area.
        /// Note that if for some reason you want the opposite behavior for either of the axis, you can make MaxScaling smaller than MinScaling.
        /// </summary>
        /// <value>The max scaling.</value>
		float MaxScaling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IScalingArea"/> should scale objects x.
        /// Usually you'll want both ScaleObjectsX and ScaleObjectsY to be on, but you can turn on only one of them
        /// to achieve a stretching effect.
        /// </summary>
        /// <value><c>true</c> if scale objects x; otherwise, <c>false</c>.</value>
		bool ScaleObjectsX { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IScalingArea"/> should scale objects y.
        /// Usually you'll want both ScaleObjectsX and ScaleObjectsY to be on, but you can turn on only one of them
        /// to achieve a stretching effect.
        /// </summary>
        /// <value><c>true</c> if scale objects x; otherwise, <c>false</c>.</value>
        bool ScaleObjectsY { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IScalingArea"/> should scale the volume.
        /// </summary>
        /// <value><c>true</c> if scale volume; otherwise, <c>false</c>.</value>
		bool ScaleVolume { get; set; }

        /// <summary>
        /// Gets or sets the axis for the scaling.
        /// If the axis is y (the default), the scaling will be adjusted based on where the object is located on
        /// the area's vertical axis (so most common use is for fake perspective).
        /// If the axis is x, the scaling will be adjusted based on where the object is located on
        /// the area's horizontal axis.
        /// </summary>
        /// <value>The axis.</value>
        ScalingAxis Axis { get; set; }

        /// <summary>
        /// Gets the scaling based on a linear interpolation of the value into the vertical area (if the axis is y)
        /// or the horizontal area (if the axis is x).
        /// When calling this method, you should pass an x value if the axis is x and a y value if the axis is y.
        /// </summary>
        /// <returns>The scaling.</returns>
        /// <param name="value">Value.</param>
        float GetScaling(float value);
	}
}

