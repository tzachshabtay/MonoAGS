using AGS.API;

namespace AGS.Engine
{
	public class AGSBoundingBoxesBuilder : IBoundingBoxBuilder
	{
        public AGSBoundingBoxesBuilder()
        {
            OnNewBoxBuildRequired = new AGSEvent();
        }

		#region AGSBoundingBoxBuilder implementation

        public IBlockingEvent OnNewBoxBuildRequired { get; private set; }

		public AGSBoundingBox BuildIntermediateBox(float width, float height, ref Matrix4 modelMatrix)
		{
			float left = 0f;
			float right = width;
			float bottom = 0f;
			float top = height;
            var bottomLeft = new Vector3(left, bottom, 0f);
            var topLeft = new Vector3(left, top, 0f);
            var bottomRight = new Vector3(right, bottom, 0f);
            var topRight = new Vector3(right, top, 0f);
            Vector3.Transform(ref bottomLeft, ref modelMatrix, out var vBottomLeft);
            Vector3.Transform(ref topLeft, ref modelMatrix, out var vTopLeft);
            Vector3.Transform(ref bottomRight, ref modelMatrix, out var vBottomRight);
            Vector3.Transform(ref topRight, ref modelMatrix, out var vTopRight);

            return new AGSBoundingBox(vBottomLeft, vBottomRight, vTopLeft, vTopRight);
		}

        public AGSBoundingBox BuildHitTestBox(ref AGSBoundingBox intermediateBox)
        {
            return buildForHitTest(ref intermediateBox);
        }

        public AGSBoundingBox BuildRenderBox(ref AGSBoundingBox intermediateBox, ref Matrix4 viewportMatrix, out PointF scale)
        {
            AGSBoundingBox renderBox = buildForRender(ref viewportMatrix, ref intermediateBox);

            var scaleX = renderBox.Width / intermediateBox.Width;
            var scaleY = renderBox.Height / intermediateBox.Height;
            scale = new PointF(scaleX, scaleY);

            return renderBox;
        }

        private AGSBoundingBox buildForRender(ref Matrix4 viewportMatrix, ref AGSBoundingBox intermediateBox)
        {
            var bottomLeft = intermediateBox.BottomLeft;
            var topLeft = intermediateBox.TopLeft;
            var bottomRight = intermediateBox.BottomRight;
            var topRight = intermediateBox.TopRight;
            Vector3.Transform(ref bottomLeft, ref viewportMatrix, out var vBottomLeft);
            Vector3.Transform(ref bottomRight, ref viewportMatrix, out var vBottomRight);
            Vector3.Transform(ref topLeft, ref viewportMatrix, out var vTopLeft);
            Vector3.Transform(ref topRight, ref viewportMatrix, out var vtopRight);
            return new AGSBoundingBox(vBottomLeft, vBottomRight, vTopLeft, vtopRight);
        }

        //Hit test box should be built without viewport transformation, and vertices need to be arranged
        //in case the sprite was flipped
        private AGSBoundingBox buildForHitTest(ref AGSBoundingBox intermediateBox)
		{
            var bottomLeft = intermediateBox.BottomLeft;
            var topLeft = intermediateBox.TopLeft;
            var bottomRight = intermediateBox.BottomRight;
            var topRight = intermediateBox.TopRight;
			if (bottomLeft.X > bottomRight.X)
			{
				if (bottomLeft.Y > topLeft.Y) //flipped on x and y
				{
                    return new AGSBoundingBox(topRight, topLeft, bottomRight, bottomLeft);
				}
				else //flipped on x
				{
                    return new AGSBoundingBox(bottomRight, bottomLeft, topRight, topLeft);
				}
			}
			else
			{
				if (bottomLeft.Y > topLeft.Y) //flipped on y
				{
                    return new AGSBoundingBox(topLeft, topRight, bottomLeft, bottomRight);
				}
				else //not flipped
				{
                    return new AGSBoundingBox(bottomLeft, bottomRight, topLeft, topRight);
				}
			}
		}
			
		#endregion
	}
}

