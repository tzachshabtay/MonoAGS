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

		private readonly IGLMatrixBuilder _matrixBuilder;
		private readonly IGLBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _textureRenderer;
		private readonly IGLBoundingBoxes _labelBoundingBoxes, _textBoundingBoxes;

		public GLLabelRenderer(Dictionary<string, GLImage> textures, IGLMatrixBuilder matrixBuilder,
			IGLBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool, 
			IGLBoundingBoxes labelBoundingBoxes, IGLBoundingBoxes textBoundingBoxes)
		{
			_matrixContainer = new MatrixContainer ();
			_textureRenderer = textureRenderer;
			_labelBoundingBoxes = labelBoundingBoxes;
			_textBoundingBoxes = textBoundingBoxes;
			_boundingBoxBuilder = boundingBoxBuilder;
			_bgRenderer = new GLImageRenderer(textures, _matrixContainer,
				_boundingBoxBuilder, colorBuilder, _textureRenderer, _labelBoundingBoxes);
			_matrixBuilder = matrixBuilder;
			_colorBuilder = colorBuilder;
			_glText = new GLText (bitmapPool, "");
			TextVisible = true;
		}

		public bool TextVisible { get; set; }
		public string Text { get; set; }
		public ITextConfig Config { get; set; }
		public bool WrapText { get; set; }
		public bool ScaleTextToFit { get; set; }
		public SizeF BaseSize { get; set; }

		#region IImageRenderer implementation

		public void Render(IObject obj, IViewport viewport)
		{
			ISprite sprite = obj.Animation.Sprite;
			IGLMatrices matrices = _matrixBuilder.Build(obj, viewport);
			_boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, matrices);
			IGLColor color = _colorBuilder.Build(obj, sprite);
			_matrixContainer.Matrices = matrices;

			_bgRenderer.Render(obj, viewport);

			color = _colorBuilder.Build(Config == null ? Color.White : Config.Color);
			if (TextVisible)
			{
				_glText.SetBatch(Text, Config == null ? null : Config.Font, WrapText ? (int?)sprite.Image.Width : null);
				_glText.Refresh();
				IGLBoundingBoxes boxes = _labelBoundingBoxes;
				if (!ScaleTextToFit)
				{
					_boundingBoxBuilder.Build(_textBoundingBoxes, _glText.Width, _glText.Height, matrices);
					boxes = _textBoundingBoxes;
				}
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
	}
}

