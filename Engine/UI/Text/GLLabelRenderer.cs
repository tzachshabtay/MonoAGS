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
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool)
		{
			_matrixContainer = new MatrixContainer ();
			_boxContainer = new BoundingBoxContainer ();
			_textureRenderer = textureRenderer;
			_bgRenderer = new GLImageRenderer(textures, _matrixContainer,
				_boxContainer, colorBuilder, _textureRenderer);
			_matrixBuilder = matrixBuilder;
			_boundingBoxBuilder = boundingBoxBuilder;
			_colorBuilder = colorBuilder;
			_glText = new GLText (bitmapPool, "");
			TextVisible = true;
		}

		public bool TextVisible { get; set; }
		public string Text { get; set; }
		public ITextConfig Config { get; set; }
		public bool WrapText { get; set; }

		#region IImageRenderer implementation

		public void Render(IObject obj, IViewport viewport)
		{
			ISprite sprite = obj.Animation.Sprite;
			IGLMatrices matrices = _matrixBuilder.Build(obj, viewport);
			IGLBoundingBoxes boxes = _boundingBoxBuilder.Build(sprite.Image.Width, sprite.Image.Height, matrices);
			IGLColor color = _colorBuilder.Build(obj, sprite);
			_matrixContainer.Matrices = matrices;
			_boxContainer.BoundingBoxes = boxes;

			_bgRenderer.Render(obj, viewport);

			color = _colorBuilder.Build(Config == null ? Color.White : Config.Color);
			if (TextVisible)
			{
				_glText.SetBatch(Text, Config == null ? null : Config.Font, WrapText ? (int?)sprite.Image.Width : null);
				_glText.Refresh();
				_textureRenderer.Render(_glText.Texture, boxes.RenderBox, color);
			}
		}

		#endregion

		private class MatrixContainer : IGLMatrixBuilder
		{
			public IGLMatrices Matrices { get; set; }

			#region IGLMatrixBuilder implementation
			public IGLMatrices Build(IObject obj, IViewport viewport)
			{
				return Matrices;
			}
			#endregion
		}

		private class BoundingBoxContainer : IGLBoundingBoxBuilder
		{
			public IGLBoundingBoxes BoundingBoxes { get; set; }

			#region IGLBoundingBoxBuilder implementation
			public IGLBoundingBoxes Build(float width, float height, IGLMatrices matrices)
			{
				return BoundingBoxes;
			}
			#endregion
		}
	}
}

