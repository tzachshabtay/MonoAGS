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

        /// <summary>
        /// Gets or sets the render box before the object was cropped.
        /// This will usually be the same as the render box unless the object was actually cropped (i.e if
        /// it had the <see cref="ICropSelfComponent"/> attached, maybe inside a scrolling panel, for example).
        /// </summary>
        /// <value>The pre crop render box.</value>
        public AGSBoundingBox PreCropRenderBox { get; set; }

        #endregion

        public bool Equals(AGSBoundingBoxes boxes)
        {
            if (this == boxes) return true;
            if (boxes == null) return false;
            if (TextureBox == null && boxes.TextureBox != null) return false;
            if (TextureBox != null && boxes.TextureBox == null) return false;
            if (TextureBox != null && !TextureBox.Equals(boxes.TextureBox)) return false;
            return RenderBox.Equals(boxes.RenderBox) && HitTestBox.Equals(boxes.HitTestBox)
                            && PreCropRenderBox.Equals(boxes.PreCropRenderBox);
        }

        public void CopyFrom(AGSBoundingBoxes boxes)
        {
            RenderBox = boxes.RenderBox;
            HitTestBox = boxes.HitTestBox;
            PreCropRenderBox = boxes.PreCropRenderBox;
            TextureBox = boxes.TextureBox;
        }
	}
}

