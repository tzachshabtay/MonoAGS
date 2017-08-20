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

		public PointF Build(AGSBoundingBoxes boxes, float width, float height, IGLMatrices matrices, bool buildRenderBox, bool buildHitTestBox)
		{
			float left = 0f;
			float right = width;
			float bottom = 0f;
			float top = height;
            Vector3 bottomLeft = Vector3.Transform(new Vector3 (left, bottom, 0f), matrices.ModelMatrix);
			Vector3 topLeft = Vector3.Transform(new Vector3 (left, top, 0f), matrices.ModelMatrix);
			Vector3 bottomRight = Vector3.Transform(new Vector3 (right, bottom, 0f), matrices.ModelMatrix);
			Vector3 topRight = Vector3.Transform(new Vector3 (right, top, 0f), matrices.ModelMatrix);

			if (buildHitTestBox) boxes.HitTestBox = buildForHitTest(bottomLeft, topLeft, bottomRight, topRight);
            if (buildRenderBox)  boxes.RenderBox = buildForRender(matrices, bottomLeft, topLeft, bottomRight, topRight);

            var scaleX = boxes.RenderBox.Width / width;
            var scaleY = boxes.RenderBox.Height / height;
            return new PointF(scaleX, scaleY);
        }

        private AGSBoundingBox buildForRender(IGLMatrices matrices, Vector3 bottomLeft, Vector3 topLeft, Vector3 bottomRight, Vector3 topRight)
        {
            return new AGSBoundingBox(Vector3.Transform(bottomLeft, matrices.ViewportMatrix),
                                      Vector3.Transform(bottomRight, matrices.ViewportMatrix),
                                      Vector3.Transform(topLeft, matrices.ViewportMatrix),
                                      Vector3.Transform(topRight, matrices.ViewportMatrix)
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

