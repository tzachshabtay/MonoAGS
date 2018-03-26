using System;

namespace AGS.API
{
    /// <summary>
    /// Represents a mouse position on the screen.
    /// Allows to get the position in either window coordinates or viewport coordinates.
    /// </summary>
    public struct MousePosition
    {
        public static Size VirtualResolution { private get; set; }
        public static Func<int> GetWindowWidth { private get; set; }
        public static Func<int> GetWindowHeight { private get; set; }

        private IViewport _mainViewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.MousePosition"/> struct.
        /// </summary>
        /// <param name="xWindow">X position of the mouse in window coordinates.</param>
        /// <param name="yWindow">Y position of the mouse in window coordinates.</param>
        /// <param name="viewport">The main viewport.</param>
        public MousePosition(float xWindow, float yWindow, IViewport viewport)
        {
            XWindow = xWindow;
            YWindow = yWindow;
            _mainViewport = viewport;
        }

        /// <summary>
        /// X position of the mouse in window coordinates.
        /// Usually this will be less helpful for interacting with the world, for that you'd want to use the viewport coordinates.
        /// </summary>
        public float XWindow { get; }

		/// <summary>
		/// Y position of the mouse in window coordinates.
		/// Usually this will be less helpful for interacting with the world, for that you'd want to use the viewport coordinates.
		/// </summary>
		public float YWindow { get; }

        /// <summary>
        /// X position of the mouse in the main viewport coordinates (i.e the world coordinates).
        /// </summary>
        public float XMainViewport => GetViewportX(_mainViewport);

        /// <summary>
        /// Y position of the mouse in the main viewport coordinates (i.e the world coordinates).
        /// </summary>
        public float YMainViewport => GetViewportY(_mainViewport);

        /// <summary>
        /// Gets the x position of the mouse in a specific viewport coordinates.
        /// </summary>
        /// <returns>The x position.</returns>
        /// <param name="viewport">Viewport.</param>
        public float GetViewportX(IViewport viewport)
        {
            float windowWidth = GetWindowWidth();
            var projectLeft = viewport.ProjectionBox.X * windowWidth;
            var projectRight = projectLeft + viewport.ProjectionBox.Width * windowWidth;
            var parentBoundingBoxes = viewport.Parent?.GetBoundingBoxes(_mainViewport);
            if (parentBoundingBoxes != null)
            {
                projectLeft += parentBoundingBoxes.ViewportBox.MinX;
                projectRight += parentBoundingBoxes.ViewportBox.MinX;
            }

            float x = MathUtils.Lerp(projectLeft, 0f, projectRight, VirtualResolution.Width, XWindow);
            return x;
        }

        /// <summary>
        /// Gets the y position of the mouse in a specific viewport coordinates.
        /// </summary>
        /// <returns>The y position.</returns>
        /// <param name="viewport">Viewport.</param>
        public float GetViewportY(IViewport viewport)
        {
            float windowHeight = GetWindowHeight();
            var projectBottom = windowHeight - viewport.ProjectionBox.Y * windowHeight;
            var projectTop = projectBottom - viewport.ProjectionBox.Height * windowHeight;
            var parentBoundingBoxes = viewport.Parent?.GetBoundingBoxes(_mainViewport);
            if (parentBoundingBoxes != null)
            {
                projectBottom -= parentBoundingBoxes.ViewportBox.MinY;
                projectTop -= parentBoundingBoxes.ViewportBox.MinY;
            }

            float y = MathUtils.Lerp(projectBottom, 0f, projectTop, VirtualResolution.Height, YWindow);
            return y;
        }

        /// <summary>
        /// Get the mouse position in a specific viewport coordinates, projected onto
        /// a specific object's world. 
        /// </summary>
        /// <returns>The projected point.</returns>
        /// <param name="viewport">Viewport.</param>
        /// <param name="projectedInto">Projected into.</param>
        public Vector2 GetProjectedPoint(IViewport viewport, IObject projectedInto)
        {
            float windowWidth = GetWindowWidth();
            float windowHeight = GetWindowHeight();
            var projectBottom = windowHeight - viewport.ProjectionBox.Y * windowHeight;
            var projectTop = projectBottom - viewport.ProjectionBox.Height * windowHeight;
            var projectLeft = viewport.ProjectionBox.X * windowWidth;
            var projectRight = projectLeft + viewport.ProjectionBox.Width * windowWidth;
            var parentBoundingBoxes = viewport.Parent?.GetBoundingBoxes(_mainViewport);
            if (parentBoundingBoxes != null)
            {
                projectLeft += parentBoundingBoxes.ViewportBox.MinX;
                projectRight += parentBoundingBoxes.ViewportBox.MinX;
                projectBottom -= parentBoundingBoxes.ViewportBox.MinY;
                projectTop -= parentBoundingBoxes.ViewportBox.MinY;
            }
            float y = MathUtils.Lerp(projectTop, VirtualResolution.Height, projectBottom, 0f, YWindow);
            float x = MathUtils.Lerp(projectLeft, 0f, projectRight, VirtualResolution.Width, XWindow);

            var matrix = viewport.GetMatrix(projectedInto.RenderLayer);
            matrix.Invert();
            Vector3 xyz = new Vector3(x, y, 0f);
            Vector3.Transform(ref xyz, ref matrix, out xyz);
            x = xyz.X;
            y = xyz.Y;

            var hitTestBox = projectedInto.TreeNode.Parent?.WorldBoundingBox;
            if (hitTestBox?.IsValid ?? false)
			{
                x -= hitTestBox.Value.MinX;
                y -= hitTestBox.Value.MinY;
			}
            var renderLayer = projectedInto.RenderLayer;
			if (renderLayer?.IndependentResolution != null)
			{
				float maxX = renderLayer.IndependentResolution.Value.Width;
				float maxY = renderLayer.IndependentResolution.Value.Height;
				x = MathUtils.Lerp(0f, 0f, VirtualResolution.Width, maxX, x);
				y = MathUtils.Lerp(0f, 0f, VirtualResolution.Height, maxY, y);
			}
            return new Vector2(x, y);
        }
    }
}