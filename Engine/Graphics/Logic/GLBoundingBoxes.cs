using System;
using OpenTK;
using AGS.API;

namespace AGS.Engine
{
	public class GLBoundingBoxes : IGLBoundingBoxes, IGLBoundingBoxBuilder
	{
		GLBoundingBox _renderBox, _hitTestBox;

		public GLBoundingBoxes()
		{
			_renderBox = new GLBoundingBox ();
			_hitTestBox = new GLBoundingBox ();
		}

		#region IGLBoundingBoxes implementation

		public IGLBoundingBox RenderBox { get { return _renderBox; }}
		public IGLBoundingBox HitTestBox{ get { return _hitTestBox;}}

		#endregion

		#region IGLBoundingBoxBuilder implementation

		public IGLBoundingBoxes Build(float width, float height, IGLMatrices matrices)
		{
			float left = 0f;
			float right = width;
			float bottom = 0f;
			float top = height;
			Vector3 bottomLeft = Vector3.Transform(new Vector3 (left, bottom, 0f), matrices.ModelMatrix);
			Vector3 topLeft = Vector3.Transform(new Vector3 (left, top, 0f), matrices.ModelMatrix);
			Vector3 bottomRight = Vector3.Transform(new Vector3 (right, bottom, 0f), matrices.ModelMatrix);
			Vector3 topRight = Vector3.Transform(new Vector3 (right, top, 0f), matrices.ModelMatrix);

			buildForHitTest(bottomLeft, topLeft, bottomRight, topRight);

			_renderBox.BottomLeft = Vector3.Transform(bottomLeft, matrices.ViewportMatrix);
			_renderBox.TopLeft = Vector3.Transform(topLeft, matrices.ViewportMatrix);
			_renderBox.BottomRight = Vector3.Transform(bottomRight, matrices.ViewportMatrix);
			_renderBox.TopRight = Vector3.Transform(topRight, matrices.ViewportMatrix);

			return this;
		}

		//Hit test box should be built before viewport transformation, and vertices need to be arranged
		//in case the sprite was flipped
		private void buildForHitTest(Vector3 bottomLeft, Vector3 topLeft, Vector3 bottomRight, Vector3 topRight)
		{
			if (bottomLeft.X > bottomRight.X)
			{
				if (bottomLeft.Y > topLeft.Y) //flipped on x and y
				{
					_hitTestBox.BottomLeft = topRight;
					_hitTestBox.BottomRight = topLeft;
					_hitTestBox.TopLeft = bottomRight;
					_hitTestBox.TopRight = bottomLeft;
				}
				else //flipped on x
				{
					_hitTestBox.BottomLeft = bottomRight;
					_hitTestBox.BottomRight = bottomLeft;
					_hitTestBox.TopLeft = topRight;
					_hitTestBox.TopRight = topLeft;
				}
			}
			else
			{
				if (bottomLeft.Y > topLeft.Y) //flipped on y
				{
					_hitTestBox.BottomLeft = topLeft;
					_hitTestBox.BottomRight = topRight;
					_hitTestBox.TopLeft = bottomLeft;
					_hitTestBox.TopRight = bottomRight;
				}
				else //not flipped
				{
					_hitTestBox.BottomLeft = bottomLeft;
					_hitTestBox.BottomRight = bottomRight;
					_hitTestBox.TopLeft = topLeft;
					_hitTestBox.TopRight = topRight;
				}
			}
		}
			
		#endregion
	}
}

