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

        public IEvent OnNewBoxBuildRequired { get; private set; }

		public AGSBoundingBox BuildIntermediateBox(float width, float height, Matrix4 modelMatrix)
		{
			float left = 0f;
			float right = width;
			float bottom = 0f;
			float top = height;
			Vector3 bottomLeft = Vector3.Transform(new Vector3(left, bottom, 0f), modelMatrix);
			Vector3 topLeft = Vector3.Transform(new Vector3(left, top, 0f), modelMatrix);
			Vector3 bottomRight = Vector3.Transform(new Vector3(right, bottom, 0f), modelMatrix);
			Vector3 topRight = Vector3.Transform(new Vector3(right, top, 0f), modelMatrix);

            return new AGSBoundingBox(bottomLeft, bottomRight, topLeft, topRight);
		}

        public AGSBoundingBox BuildHitTestBox(AGSBoundingBox intermediateBox)
        {
            return buildForHitTest(intermediateBox.BottomLeft, intermediateBox.TopLeft, 
                                   intermediateBox.BottomRight, intermediateBox.TopRight);
        }

        public AGSBoundingBox BuildRenderBox(AGSBoundingBox intermediateBox, Matrix4 viewportMatrix, out PointF scale)
        {
            AGSBoundingBox renderBox = buildForRender(viewportMatrix, intermediateBox.BottomLeft, intermediateBox.TopLeft,
                                                      intermediateBox.BottomRight, intermediateBox.TopRight);

            var scaleX = renderBox.Width / intermediateBox.Width;
            var scaleY = renderBox.Height / intermediateBox.Height;
            scale = new PointF(scaleX, scaleY);

            return renderBox;
        }

		private AGSBoundingBox buildForRender(Matrix4 viewportMatrix, Vector3 bottomLeft, Vector3 topLeft, Vector3 bottomRight, Vector3 topRight)
        {
            return new AGSBoundingBox(Vector3.Transform(bottomLeft, viewportMatrix),
                                      Vector3.Transform(bottomRight, viewportMatrix),
                                      Vector3.Transform(topLeft, viewportMatrix),
                                      Vector3.Transform(topRight, viewportMatrix)
                                     );
        }

        //Hit test box should be built without viewport transformation, and vertices need to be arranged
        //in case the sprite was flipped
        private AGSBoundingBox buildForHitTest(Vector3 bottomLeft, Vector3 topLeft, Vector3 bottomRight, Vector3 topRight)
		{
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

