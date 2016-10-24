namespace AGS.Engine
{
	public class GLBoundingBoxesBuilder : IGLBoundingBoxBuilder
	{
		#region IGLBoundingBoxBuilder implementation

		public void Build(IGLBoundingBoxes boxes, float width, float height, IGLMatrices matrices, bool buildRenderBox, bool buildHitTestBox)
		{
			float left = 0f;
			float right = width;
			float bottom = 0f;
			float top = height;
            Vector3 bottomLeft = Vector3.Transform(new Vector3 (left, bottom, 0f), matrices.ModelMatrix);
			Vector3 topLeft = Vector3.Transform(new Vector3 (left, top, 0f), matrices.ModelMatrix);
			Vector3 bottomRight = Vector3.Transform(new Vector3 (right, bottom, 0f), matrices.ModelMatrix);
			Vector3 topRight = Vector3.Transform(new Vector3 (right, top, 0f), matrices.ModelMatrix);

			if (buildHitTestBox) buildForHitTest(boxes.HitTestBox, bottomLeft, topLeft, bottomRight, topRight);
            if (buildRenderBox)  buildForRender(boxes.RenderBox, matrices, bottomLeft, topLeft, bottomRight, topRight);            
		}

        private void buildForRender(IGLBoundingBox renderBox, IGLMatrices matrices, Vector3 bottomLeft, Vector3 topLeft, Vector3 bottomRight, Vector3 topRight)
        {
            renderBox.BottomLeft = Vector3.Transform(bottomLeft, matrices.ViewportMatrix);
            renderBox.TopLeft = Vector3.Transform(topLeft, matrices.ViewportMatrix);
            renderBox.BottomRight = Vector3.Transform(bottomRight, matrices.ViewportMatrix);
            renderBox.TopRight = Vector3.Transform(topRight, matrices.ViewportMatrix);
        }

        //Hit test box should be built without viewport transformation, and vertices need to be arranged
        //in case the sprite was flipped
        private void buildForHitTest(IGLBoundingBox hitTestBox, Vector3 bottomLeft, Vector3 topLeft, Vector3 bottomRight, Vector3 topRight)
		{
			if (bottomLeft.X > bottomRight.X)
			{
				if (bottomLeft.Y > topLeft.Y) //flipped on x and y
				{
					hitTestBox.BottomLeft = topRight;
					hitTestBox.BottomRight = topLeft;
					hitTestBox.TopLeft = bottomRight;
					hitTestBox.TopRight = bottomLeft;
				}
				else //flipped on x
				{
					hitTestBox.BottomLeft = bottomRight;
					hitTestBox.BottomRight = bottomLeft;
					hitTestBox.TopLeft = topRight;
					hitTestBox.TopRight = topLeft;
				}
			}
			else
			{
				if (bottomLeft.Y > topLeft.Y) //flipped on y
				{
					hitTestBox.BottomLeft = topLeft;
					hitTestBox.BottomRight = topRight;
					hitTestBox.TopLeft = bottomLeft;
					hitTestBox.TopRight = bottomRight;
				}
				else //not flipped
				{
					hitTestBox.BottomLeft = bottomLeft;
					hitTestBox.BottomRight = bottomRight;
					hitTestBox.TopLeft = topLeft;
					hitTestBox.TopRight = topRight;
				}
			}
		}
			
		#endregion
	}
}

