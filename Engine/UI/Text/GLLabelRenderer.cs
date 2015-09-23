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
		private readonly IGLViewportMatrix _viewport;

		public GLLabelRenderer(Dictionary<string, GLImage> textures, IGLMatrixBuilder matrixBuilder,
			IGLBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool, IGLViewportMatrix viewportMatrix,
			IGLBoundingBoxes labelBoundingBoxes, IGLBoundingBoxes textBoundingBoxes)
		{
			_matrixContainer = new MatrixContainer ();
			_viewport = viewportMatrix;
			_textureRenderer = textureRenderer;
			_labelBoundingBoxes = labelBoundingBoxes;
			_textBoundingBoxes = textBoundingBoxes;
			_boundingBoxBuilder = boundingBoxBuilder;
			_bgRenderer = new GLImageRenderer(textures, _matrixContainer,
				new BoundingBoxesEmptyBuilder(), colorBuilder, _textureRenderer, _labelBoundingBoxes, _viewport);
			_matrixBuilder = matrixBuilder;
			_colorBuilder = colorBuilder;
			_glText = new GLText (bitmapPool);
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
			IGLMatrices matrices = _matrixBuilder.Build(obj, _viewport.GetMatrix(viewport));
			_boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, matrices);
			IGLColor color = _colorBuilder.Build(obj, sprite);
			_matrixContainer.Matrices = matrices;

			_bgRenderer.Render(obj, viewport);

			if (TextVisible)
			{
				SizeF size = new SizeF (_labelBoundingBoxes.RenderBox.Width, _labelBoundingBoxes.RenderBox.Height);
				_glText.SetProperties(size, Text, Config, WrapText ? (int?)sprite.Image.Width : null);
				_glText.Refresh();
				IGLBoundingBoxes boxes = _labelBoundingBoxes;
				if (!ScaleTextToFit)
				{
					_boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, _glText.BitmapHeight, matrices);
					boxes = _textBoundingBoxes;
				}
				color = _colorBuilder.Build(Color.White);
				_textureRenderer.Render(_glText.Texture, boxes.RenderBox, color);
			}
		}
			
		#endregion

		private class MatrixContainer : IGLMatrixBuilder
		{
			public IGLMatrices Matrices { get; set; }

			#region IGLMatrixBuilder implementation
			public IGLMatrices Build(IObject obj, Matrix4 viewport)
			{
				return Matrices;
			}
			#endregion
		}

		private class BoundingBoxesEmptyBuilder : IGLBoundingBoxBuilder
		{
			#region IGLBoundingBoxBuilder implementation
			public void Build(IGLBoundingBoxes boxes, float width, float height, IGLMatrices matrices)
			{
			}
			#endregion
			
		}
	}
}

