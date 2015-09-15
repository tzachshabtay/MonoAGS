using System;
using AGS.API;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class GLLabelRenderer : ILabelRenderer
	{
		private readonly GLImageRenderer _bgRenderer;
		private readonly GLText _glText;

		private readonly MatrixContainer _matrixContainer;
		private readonly BoundingBoxContainer _boxContainer;

		private readonly IGLMatrixBuilder _matrixBuilder;
		private readonly IGLBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _textureRenderer;

		public GLLabelRenderer(Dictionary<string, GLImage> textures, IGLMatrixBuilder matrixBuilder,
			IGLBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer)
		{
			_matrixContainer = new MatrixContainer ();
			_boxContainer = new BoundingBoxContainer ();
			_textureRenderer = textureRenderer;
			_bgRenderer = new GLImageRenderer(textures, _matrixContainer,
				_boxContainer, colorBuilder, _textureRenderer);
			_matrixBuilder = matrixBuilder;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_glText = new GLText ("") { Visible = true };
		}

		public string Text { get; set; }
		public ITextConfig Config { get; set; }

		#region IImageRenderer implementation

		public void Render(IObject obj, IViewport viewport)
		{
			ISprite sprite = obj.Animation.Sprite;
			Matrix4 matrix = _matrixBuilder.Build(obj, viewport);
			IGLBoundingBox box = _boundingBoxBuilder.Build(sprite.Image.Width, sprite.Image.Height, matrix);
			IGLColor color = _colorBuilder.Build(obj, sprite);
			_matrixContainer.Matrix = matrix;
			_boxContainer.BoundingBox = box;

			_bgRenderer.Render(obj, viewport);

			color = _colorBuilder.Build(Config == null ? Color.White : Config.Color);
			if (_glText.Visible)
			{
				_glText.Text = Text;
				_glText.Refresh();
				_textureRenderer.Render(_glText.Texture, box, color);
			}
		}

		#endregion

		private class MatrixContainer : IGLMatrixBuilder
		{
			public Matrix4 Matrix { get; set; }

			#region IGLMatrixBuilder implementation
			public Matrix4 Build(IObject obj, IViewport viewport)
			{
				return Matrix;
			}
			#endregion
		}

		private class BoundingBoxContainer : IGLBoundingBoxBuilder
		{
			public IGLBoundingBox BoundingBox { get; set; }

			#region IGLBoundingBoxBuilder implementation
			public IGLBoundingBox Build(float width, float height, Matrix4 matrix)
			{
				return BoundingBox;
			}
			#endregion
		}
	}
}

