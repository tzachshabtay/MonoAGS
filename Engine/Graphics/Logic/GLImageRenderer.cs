using System;
using API;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK;

namespace Engine
{
	public class GLImageRenderer : IImageRenderer
	{
		private Dictionary<string, GLImage> _textures;
		private IGLMatrixBuilder _matrixBuilder;
		private IGLBoundingBoxBuilder _boundingBoxBuilder;
		private IGLColorBuilder _colorBuilder;
		private IGLTextureRenderer _renderer;

		public GLImageRenderer (Dictionary<string, GLImage> textures, 
			IGLMatrixBuilder matrixBuilder, IGLBoundingBoxBuilder boundingBoxBuilder,
			IGLColorBuilder colorBuilder, IGLTextureRenderer renderer)
		{
			_textures = textures;
			_matrixBuilder = matrixBuilder;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_renderer = renderer;
		}

		/*private Matrix4 getModelMatrix(ISprite sprite)
		{
			IPoint anchorOffsets = getAnchorOffsets (sprite.Anchor, sprite.Width, sprite.Height);
			Matrix4 anchor = Matrix4.CreateTranslation (new Vector3(-anchorOffsets.X, -anchorOffsets.Y, 0f));
			Matrix4 scale = Matrix4.CreateScale (new Vector3 (sprite.ScaleX, sprite.ScaleY, 1f));
			//Quaternion q = Quaternion.FromAxisAngle (Vector3.UnitZ, sprite.Angle);
			//Matrix4 rotation = Matrix4.CreateFromQuaternion (q);
			Matrix4 rotation = Matrix4.CreateRotationZ(sprite.Angle);
			Matrix4 transform = Matrix4.CreateTranslation (new Vector3(sprite.X, sprite.Y, 0f));
			return anchor * scale * rotation * transform;
		}*/

		public void Render(IObject obj, IViewport viewport)
		{
			ISprite sprite = obj.Animation.Sprite;
			/*Matrix4 spriteMatrix = getModelMatrix (sprite);
			Matrix4 objMatrix = getModelMatrix (obj);

			Matrix4 modelMatrix = spriteMatrix * objMatrix;
			IObject parent = obj.TreeNode.Parent;
			while (parent != null)
			{
				Matrix4 parentMatrix = getModelMatrix(parent);
				modelMatrix = modelMatrix * parentMatrix;
				parent = parent.TreeNode.Parent;
			}
			Matrix4 viewMatrix = obj.IgnoreViewport ? Matrix4.Identity :
				Matrix4.CreateScale(viewport.ScaleX, viewport.ScaleY, 1f) *
				Matrix4.CreateTranslation(new Vector3(-viewport.X, -viewport.Y, 0f));

			Matrix4 mvMatrix = modelMatrix * viewMatrix;*/
			Matrix4 mvMatrix = _matrixBuilder.Build(obj, viewport);

			/*float left = 0f;
			float right = sprite.Image.Width;
			float bottom = 0f;
			float top = sprite.Image.Height;
			Vector3 bottomLeft = Vector3.Transform(new Vector3 (left, bottom, 0f), mvMatrix);
			Vector3 topLeft = Vector3.Transform(new Vector3 (left, top, 0f), mvMatrix);
			Vector3 bottomRight = Vector3.Transform(new Vector3 (right, bottom, 0f), mvMatrix);
			Vector3 topRight = Vector3.Transform(new Vector3 (right, top, 0f), mvMatrix);*/
			IGLBoundingBox boundingBox = _boundingBoxBuilder.Build(sprite.Image.Width,
				                             sprite.Image.Height, mvMatrix);
			Vector3 bottomLeft = boundingBox.BottomLeft;
			Vector3 topLeft = boundingBox.TopLeft;
			Vector3 bottomRight = boundingBox.BottomRight;
			Vector3 topRight = boundingBox.TopRight;

			GLImage glImage = _textures.GetOrAdd (sprite.Image.ID, () => createNewTexture (sprite.Image.ID));

			/*float tintRed = multiply (s => s.Tint.R/255f, sprite, obj);
			float tintGreen = multiply (s => s.Tint.G/255f, sprite, obj);
			float tintBlue = multiply (s => s.Tint.B/255f, sprite, obj);
			float opacity = multiply (s => s.Opacity/255f, sprite, obj);*/
			IGLColor color = _colorBuilder.Build(sprite, obj);

			//GLUtils.DrawQuad (glImage.Texture, bottomLeft, bottomRight, topLeft, topRight, color.R,
			//	color.G, color.B, color.A);
			_renderer.Render(glImage.Texture, boundingBox, color);

			AGSSquare square = new AGSSquare (new AGSPoint (bottomLeft.X, bottomLeft.Y),
				                   new AGSPoint (bottomRight.X, bottomRight.Y), new AGSPoint (topLeft.X, topLeft.Y),
				                   new AGSPoint (topRight.X, topRight.Y));
			obj.BoundingBox = square;

			IBorderStyle border = obj.Border;
			if (border != null)
			{
				color = _colorBuilder.Build(border.Color);
				GLUtils.DrawQuadBorder(bottomLeft, bottomRight, topLeft, topRight, border.LineWidth, 
					color.R, color.G, color.B, color.A);
			}
			if (obj.DebugDrawAnchor)
				GLUtils.DrawCross (obj.X - viewport.X, obj.Y - viewport.Y, 10, 10, 1f, 1f, 1f, 1f);
		}
			
		/*private IPoint getAnchorOffsets(IPoint anchor, float width, float height)
		{
			float x = MathUtils.Lerp (0f, 0f, 1f, width, anchor.X);
			float y = MathUtils.Lerp (0f, 0f, 1f, height, anchor.Y);
			return new AGSPoint (x, y);
		}*/

		private GLImage createNewTexture(string path)
		{
			if (string.IsNullOrEmpty(path)) return new GLImage () { Width = 1, Height = 1 }; //transparent image

			GLGraphicsFactory loader = new GLGraphicsFactory (null);
			return loader.LoadImageInner (path);
		}

		/*private float multiply(Func<ISprite, float> getter, params ISprite[] sprites)
		{
			return process ((arg1, arg2) => arg1 * arg2, getter, sprites);
		}

		private float process(Func<float, float, float> apply, Func<ISprite, float> getter, params ISprite[] sprites)
		{
			float result = float.NaN;
			bool firstIteration = true;
			foreach (ISprite sprite in sprites) 
			{
				if (sprite == null)
					continue;
				float prop = getter (sprite);
				if (firstIteration) 
				{
					result = prop;
					firstIteration = false;
				}
				else
					result = apply (result, prop);
			}
			return result;
		}*/
	}
}

