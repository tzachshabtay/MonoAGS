using AGS.API;

namespace AGS.Engine
{
	public class GLMatrixBuilder : IGLMatrixBuilder, IGLMatrices
	{
		public GLMatrixBuilder()
		{
		}

		public static readonly PointF NoScaling = new PointF(1f,1f);

		#region IGLMatrices implementation

		public Matrix4 ModelMatrix { get; private set; }

		public Matrix4 ViewportMatrix { get; private set; }

		#endregion

		//http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/
		//http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-17-quaternions/
		public IGLMatrices Build(IHasModelMatrix obj, IHasModelMatrix sprite, IObject parent, Matrix4 viewport, PointF areaScaling, PointF resolutionTransform)
		{
            Matrix4 spriteMatrix = getModelMatrix (sprite, NoScaling, NoScaling);
			Matrix4 objMatrix = getModelMatrix (obj, areaScaling, resolutionTransform);

			ModelMatrix = spriteMatrix * objMatrix;
			while (parent != null)
			{
				Matrix4 parentMatrix = getModelMatrix(parent, NoScaling, resolutionTransform);
				ModelMatrix = ModelMatrix * parentMatrix;
				parent = parent.TreeNode.Parent;
			}
			ViewportMatrix = viewport;

			return this;		
		}

		private Matrix4 getModelMatrix(IHasModelMatrix sprite, PointF areaScaling, PointF resolutionTransform)
		{
            if (sprite == null) return Matrix4.Identity;
            float width = sprite.Width * resolutionTransform.X;
            float height = sprite.Height * resolutionTransform.Y;
            PointF anchorOffsets = getAnchorOffsets (sprite.Anchor, width, height);
			Matrix4 anchor = Matrix4.CreateTranslation (new Vector3(-anchorOffsets.X, -anchorOffsets.Y, 0f));
			Matrix4 scale = Matrix4.CreateScale (new Vector3 (sprite.ScaleX * areaScaling.X, 
				sprite.ScaleY * areaScaling.Y, 1f));
			Matrix4 rotation = Matrix4.CreateRotationZ(sprite.Angle);
            float x = sprite.X * resolutionTransform.X;
            float y = sprite.Y * resolutionTransform.Y;
            Matrix4 transform = Matrix4.CreateTranslation (new Vector3(x, y, 0f));
			return anchor * scale * rotation * transform;
		}

        private PointF getAnchorOffsets(PointF anchor, float width, float height)
		{
			float x = MathUtils.Lerp (0f, 0f, 1f, width, anchor.X);
			float y = MathUtils.Lerp (0f, 0f, 1f, height, anchor.Y);
			return new PointF (x, y);
		}
	}
}

