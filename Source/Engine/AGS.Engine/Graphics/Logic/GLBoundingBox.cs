using System;
using AGS.API;

namespace AGS.Engine
{
	public class GLBoundingBox : IGLBoundingBox
	{
		public GLBoundingBox()
		{
		}

		#region IGLBoundingBox implementation

		public Vector3 BottomLeft { get; set; }

		public Vector3 TopLeft { get; set; }

		public Vector3 BottomRight { get; set; }

		public Vector3 TopRight { get; set; }

		public float Width { get { return distance(BottomLeft, BottomRight); }}
		public float Height { get { return distance(BottomLeft, TopLeft); }}

		public AGSSquare ToSquare()
		{
			AGSSquare square = new AGSSquare (new PointF (BottomLeft.X, BottomLeft.Y), new PointF (BottomRight.X, BottomRight.Y),
				new PointF (TopLeft.X, TopLeft.Y), new PointF (TopRight.X, TopRight.Y));
			if (square.BottomLeft.X > square.BottomRight.X) square = square.FlipHorizontal();
			return square;
		}

        public FourCorners<Vector2> Crop(ICropSelfComponent crop, PointF resolutionFactor, PointF adjustedScale)
        {
            if (crop == null || !crop.CropEnabled) return null;
            float width, height;
            float scaleX = resolutionFactor.X * adjustedScale.X;
            float scaleY = resolutionFactor.Y * adjustedScale.Y;
            var cropArea = AGSCropSelfComponent.GetCropArea(crop, Width / scaleX, Height / scaleY,
                out width, out height);
            width *= scaleX;
            height *= scaleY;

            float boxWidth = Width;
            float boxHeight = Height;
            float rightForTopRight = MathUtils.Lerp(0f, TopLeft.X, boxWidth, TopRight.X, width);
            float rightForBottomRight = MathUtils.Lerp(0f, BottomLeft.X, boxWidth, BottomRight.X, width);
            float bottomForBottomLeft = MathUtils.Lerp(0f, TopLeft.Y, boxHeight, BottomLeft.Y, height);
            float bottomForBottomRight = MathUtils.Lerp(0f, TopRight.Y, boxHeight, BottomRight.Y, height);
            float topForTopRight = MathUtils.Lerp(0f, Math.Min(TopLeft.Y, TopRight.Y), boxWidth, Math.Max(TopLeft.Y, TopRight.Y), width);
            float leftForBottomLeft = MathUtils.Lerp(0f, Math.Min(TopLeft.X, BottomLeft.X), boxHeight, Math.Max(TopLeft.X, BottomLeft.X), height);
            float offsetX = crop.CropArea.X * scaleX;
            float offsetY = crop.CropArea.Y * scaleY;
            TopLeft = new Vector3(TopLeft.X + offsetX, TopLeft.Y - offsetY, TopLeft.Z);
            TopRight = new Vector3(rightForTopRight + offsetX, topForTopRight - offsetY, TopRight.Z);
            BottomLeft = new Vector3(leftForBottomLeft + offsetX, bottomForBottomLeft - offsetY, BottomLeft.Z);
            BottomRight = new Vector3(rightForBottomRight + offsetX, bottomForBottomRight - offsetY, BottomRight.Z);
            return cropArea;
        }
			
		#endregion

		private float distance(Vector3 a, Vector3 b)
		{
			float dx = a.X - b.X;
			float dy = a.Y - b.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}
	}
}

