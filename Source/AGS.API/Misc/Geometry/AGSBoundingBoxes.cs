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
        /// Gets or sets the bounding box in viewport coordinages (used for rendering the entity).
        /// This should be only set from the engine.
        /// </summary>
        /// <value>The render box.</value>
        public AGSBoundingBox ViewportBox { get; set; }

        /// <summary>
        /// Gets or sets the bounding box in world coordinates (used for collision checks).
        /// This should be only set from the engine.
        /// </summary>
        /// <value>The hit-test box.</value>
        public AGSBoundingBox WorldBox { get; set; }

        /// <summary>
        /// Gets or sets the texture box which specifies which part of the texture is used on the render box.
        /// Usually this would be the entire texture: (0,0)-(1,1), unless cropped by a cropping component.
        /// This should be only set from the engine.
        /// </summary>
        /// <value>The texture box.</value>
        public FourCorners<Vector2> TextureBox { get; set; }

        /// <summary>
        /// Gets or sets the viewport bounding box before the object was cropped.
        /// This will usually be the same as the viewport box unless the object was actually cropped (i.e if
        /// it had the <see cref="ICropSelfComponent"/> attached, maybe inside a scrolling panel, for example).
        /// </summary>
        /// <value>The pre crop render box.</value>
        public AGSBoundingBox PreCropViewportBox { get; set; }

        #endregion

        public bool Equals(AGSBoundingBoxes boxes)
        {
            if (this == boxes) return true;
            if (boxes == null) return false;
            if (TextureBox == null && boxes.TextureBox != null) return false;
            if (TextureBox != null && boxes.TextureBox == null) return false;
            if (TextureBox != null && !TextureBox.Equals(boxes.TextureBox)) return false;
            return ViewportBox.Equals(boxes.ViewportBox) && WorldBox.Equals(boxes.WorldBox)
                            && PreCropViewportBox.Equals(boxes.PreCropViewportBox);
        }

        public void CopyFrom(AGSBoundingBoxes boxes)
        {
            ViewportBox = boxes.ViewportBox;
            WorldBox = boxes.WorldBox;
            PreCropViewportBox = boxes.PreCropViewportBox;
            TextureBox = boxes.TextureBox;
        }
	}
}

