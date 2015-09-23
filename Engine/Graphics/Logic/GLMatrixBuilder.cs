﻿using System;
using OpenTK;
using AGS.API;

namespace AGS.Engine
{
	public class GLMatrixBuilder : IGLMatrixBuilder, IGLMatrices
	{
		public GLMatrixBuilder()
		{
		}

		#region IGLMatrices implementation

		public Matrix4 ModelMatrix { get; private set; }

		public Matrix4 ViewportMatrix { get; private set; }

		#endregion

		//http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/
		//http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-17-quaternions/
		public IGLMatrices Build(IObject obj, Matrix4 viewport)
		{
			ISprite sprite = obj.Animation.Sprite;
			Matrix4 spriteMatrix = getModelMatrix (sprite);
			Matrix4 objMatrix = getModelMatrix (obj);

			ModelMatrix = spriteMatrix * objMatrix;
			IObject parent = obj.TreeNode.Parent;
			while (parent != null)
			{
				Matrix4 parentMatrix = getModelMatrix(parent);
				ModelMatrix = ModelMatrix * parentMatrix;
				parent = parent.TreeNode.Parent;
			}
			ViewportMatrix = obj.IgnoreViewport ? Matrix4.Identity :
				viewport;

			return this;		
		}

		private Matrix4 getModelMatrix(ISprite sprite)
		{
			IPoint anchorOffsets = getAnchorOffsets (sprite.Anchor, sprite.Width, sprite.Height);
			Matrix4 anchor = Matrix4.CreateTranslation (new Vector3(-anchorOffsets.X, -anchorOffsets.Y, 0f));
			Matrix4 scale = Matrix4.CreateScale (new Vector3 (sprite.ScaleX, sprite.ScaleY, 1f));
			//Quaternion q = Quaternion.FromAxisAngle (Vector3.UnitZ, sprite.Angle);
			//Matrix4 rotation = Matrix4.CreateFromQuaternion (q);
			Matrix4 rotation = Matrix4.CreateRotationZ(sprite.Angle);
			Matrix4 transform = Matrix4.CreateTranslation (new Vector3(sprite.X, sprite.Y, 0f));
			return anchor * scale * rotation * transform;
		}

		private IPoint getAnchorOffsets(IPoint anchor, float width, float height)
		{
			float x = MathUtils.Lerp (0f, 0f, 1f, width, anchor.X);
			float y = MathUtils.Lerp (0f, 0f, 1f, height, anchor.Y);
			return new AGSPoint (x, y);
		}
	}
}

