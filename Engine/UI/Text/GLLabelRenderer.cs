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
		private GLText _glText;

		private readonly MatrixContainer _matrixContainer;

		private readonly IGLMatrixBuilder _matrixBuilder;
		private readonly IGLBoundingBoxBuilder _boundingBoxBuilder;
		private readonly IGLColorBuilder _colorBuilder;
		private readonly IGLTextureRenderer _textureRenderer;
		private readonly IGLBoundingBoxes _labelBoundingBoxes, _textBoundingBoxes;
		private readonly IGLViewportMatrix _viewport;
		private IGLBoundingBoxes _usedLabelBoundingBoxes, _usedTextBoundingBoxes;
		private readonly LabelMatrixRenderTarget _labelMatrixRenderTarget;
		private readonly BitmapPool _bitmapPool;

		public GLLabelRenderer(Dictionary<string, GLImage> textures, IGLMatrixBuilder matrixBuilder,
			IGLBoundingBoxBuilder boundingBoxBuilder, IGLColorBuilder colorBuilder, 
			IGLTextureRenderer textureRenderer, BitmapPool bitmapPool, IGLViewportMatrix viewportMatrix,
			IGLBoundingBoxes labelBoundingBoxes, IGLBoundingBoxes textBoundingBoxes)
		{
			_matrixContainer = new MatrixContainer ();
			_bitmapPool = bitmapPool;
			_labelMatrixRenderTarget = new LabelMatrixRenderTarget ();
			_viewport = viewportMatrix;
			_textureRenderer = textureRenderer;
			_labelBoundingBoxes = labelBoundingBoxes;
			_textBoundingBoxes = textBoundingBoxes;
			_boundingBoxBuilder = boundingBoxBuilder;
			_bgRenderer = new GLImageRenderer(textures, _matrixContainer,
				new BoundingBoxesEmptyBuilder(), colorBuilder, _textureRenderer, _labelBoundingBoxes,
				viewportMatrix);
			_matrixBuilder = matrixBuilder;
			_colorBuilder = colorBuilder;

			TextVisible = true;
		}

		public bool TextVisible { get; set; }
		public string Text { get; set; }
		public ITextConfig Config { get; set; }
		public SizeF BaseSize { get; set; }
		public ILabel Label { get; set; }


		public float Width 
		{ 
			get 
			{ 
				return _usedLabelBoundingBoxes == null || _usedLabelBoundingBoxes.RenderBox == null ? 1f : _usedLabelBoundingBoxes.RenderBox.Width; 
			}
		}

		public float Height 
		{ 
			get 
			{ 
				return _usedLabelBoundingBoxes == null || _usedLabelBoundingBoxes.RenderBox == null ? 1f :  _usedLabelBoundingBoxes.RenderBox.Height; 
			}
		}

		public float TextWidth 
		{ 
			get 
			{ 
				return _glText == null ? 1f : _glText.Width; 
			}
		}

		public float TextHeight 
		{ 
			get 
			{ 
				return _glText == null ? 1f : _glText.Height;
			}
		}

		#region IImageRenderer implementation

		public void Prepare(IObject obj, IViewport viewport, IPoint areaScaling)
		{
			_glText = _glText ?? new GLText (_bitmapPool);

			updateBoundingBoxes(obj, viewport, areaScaling);
			_bgRenderer.BoundingBoxes = _usedLabelBoundingBoxes;
			_bgRenderer.Prepare(obj, viewport, areaScaling);
		}

		public void Render(IObject obj, IViewport viewport, IPoint areaScaling)
		{
			_bgRenderer.Render(obj, viewport, areaScaling);

			if (TextVisible)
			{
				IGLColor color = _colorBuilder.Build(Color.White);
				_textureRenderer.Render(_glText.Texture, _usedTextBoundingBoxes.RenderBox, color);
			}
		}

		#endregion

		private void updateBoundingBoxes(IObject obj, IViewport viewport, IPoint areaScaling)
		{
			ITextConfig config = Config;
			AutoFit autoFit = TextVisible && config != null ? config.AutoFit : AutoFit.NoFitting;
			updateLabelMatrixRenderTarget(obj);

			float height = obj.Height;
			float width = obj.Width;
			if (autoFit == AutoFit.LabelShouldFitText)
			{
				updateText(GLText.EmptySize, null);
				_labelMatrixRenderTarget.Width = _glText.Width;
				_labelMatrixRenderTarget.Height = _glText.Height;
			}
			IGLMatrices matrices = _matrixBuilder.Build(_labelMatrixRenderTarget, obj.Animation.Sprite, obj.TreeNode.Parent,
				obj.IgnoreViewport ? Matrix4.Identity : _viewport.GetMatrix(viewport), areaScaling);
			_matrixContainer.Matrices = matrices;
			_labelMatrixRenderTarget.Width = width;
			_labelMatrixRenderTarget.Height = height;

			switch (autoFit)
			{
				case AutoFit.NoFitting:
					_boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, matrices);
					updateText(new SizeF (_labelBoundingBoxes.RenderBox.Width, _labelBoundingBoxes.RenderBox.Height),
						null);
					_boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, _glText.BitmapHeight, matrices);

					_usedLabelBoundingBoxes = _labelBoundingBoxes;
					_usedTextBoundingBoxes = _textBoundingBoxes;
					break;

				case AutoFit.TextShouldWrap:
					ISprite sprite = obj.Animation.Sprite;
					_boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, matrices);
					updateText(GLText.EmptySize, (int?)BaseSize.Width);
					_boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, _glText.BitmapHeight, matrices);

					_usedLabelBoundingBoxes = _labelBoundingBoxes;
					_usedTextBoundingBoxes = _textBoundingBoxes;
					break;

				case AutoFit.TextShouldFitLabel:
					_boundingBoxBuilder.Build(_labelBoundingBoxes, BaseSize.Width, BaseSize.Height, matrices);
					updateText(GLText.EmptySize, null);
					
					float textWidth = MathUtils.Lerp(0f, 0f, _glText.Width, BaseSize.Width,  _glText.BitmapWidth);
					float textHeight = MathUtils.Lerp(0f, 0f, _glText.Height, BaseSize.Height, _glText.BitmapHeight);
					_boundingBoxBuilder.Build(_textBoundingBoxes, textWidth, textHeight, matrices);

					_usedLabelBoundingBoxes = _labelBoundingBoxes;
					_usedTextBoundingBoxes = _textBoundingBoxes;
					break;

				case AutoFit.LabelShouldFitText:
					_boundingBoxBuilder.Build(_textBoundingBoxes, _glText.BitmapWidth, _glText.BitmapHeight, matrices);
					_boundingBoxBuilder.Build(_labelBoundingBoxes, _glText.Width, _glText.Height, matrices);

					_usedLabelBoundingBoxes = _labelBoundingBoxes;
					_usedTextBoundingBoxes = _textBoundingBoxes;
					break;

				default:
					throw new NotSupportedException (autoFit.ToString());
			}
		}

		private void updateLabelMatrixRenderTarget(IObject obj)
		{
			_labelMatrixRenderTarget.X = obj.X;
			_labelMatrixRenderTarget.Y = obj.Y;
			_labelMatrixRenderTarget.Anchor = obj.Anchor;
			_labelMatrixRenderTarget.Angle = obj.Angle;
			_labelMatrixRenderTarget.Width = BaseSize.Width;
			_labelMatrixRenderTarget.Height = BaseSize.Height;
			_labelMatrixRenderTarget.ScaleX = obj.ScaleX;
			_labelMatrixRenderTarget.ScaleY = obj.ScaleY;
		}

		private void updateText(SizeF baseSize, int? maxWidth)
		{
			if (TextVisible)
			{
				_glText.SetProperties(baseSize, Text, Config, maxWidth);
				_glText.Refresh();
			}
		}

		private class MatrixContainer : IGLMatrixBuilder
		{
			public IGLMatrices Matrices { get; set; }

			#region IGLMatrixBuilder implementation
			public IGLMatrices Build(ISprite obj, ISprite sprite, IObject parent, Matrix4 viewport, IPoint areaScaling)
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