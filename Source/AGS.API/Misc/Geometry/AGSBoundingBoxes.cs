namespace AGS.API
{
    /// <summary>
    /// The bounding boxes used for the entity.
    /// </summary>
	public class AGSBoundingBoxes
	{
        //todo: make those properties readonly, should only be set by the engine, should be in the API

		#region AGSBoundingBoxes implementation

        /// <summary>
        /// Gets or sets the render box used for rendering the entity.
        /// This should be only set from the engine.
        /// </summary>
        /// <value>The render box.</value>
		public AGSBoundingBox RenderBox { get; set; }

		/// <summary>
		/// Gets or sets the hit-test box used for collision checks.
		/// This should be only set from the engine.
		/// </summary>
		/// <value>The hit-test box.</value>
		public AGSBoundingBox HitTestBox { get; set; }

		/// <summary>
		/// Gets or sets the texture box which specifies which part of the texture is used on the render box.
        /// Usually this would be the entire texture: (0,0)-(1,1), unless cropped by a cropping component.
		/// This should be only set from the engine.
		/// </summary>
		/// <value>The texture box.</value>
		public FourCorners<Vector2> TextureBox { get; set; }

		#endregion
	}
}

