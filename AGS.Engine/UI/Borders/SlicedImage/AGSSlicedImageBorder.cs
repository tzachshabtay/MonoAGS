using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
	public enum BorderRepeat
	{
		Stretch,
		Repeat,
		Round,
		Space,
	}

	//Inspired from css: https://css-tricks.com/almanac/properties/b/border-image/
	public class AGSSlicedImageBorder : IBorderStyle
	{
		private int _texture;
		private float _width, _height;
		private readonly IGLColor _white;

		public AGSSlicedImageBorder()
		{
			_white = Colors.White.ToGLColor();
		}

		public IAnimation Image { get; set; }

		public SliceValues Slice { get; set; }

		public SliceValues Width { get; set; }

		public SliceValues Outset { get; set; }

		public BorderRepeat Repeat { get; set; }

		public bool FillBackground { get; set; }

		public bool DrawBorderBehind { get; set; }

        public float WidthLeft { get { return Width.Left.ToPixels(_width).Value; } }
        public float WidthRight { get { return Width.Right.ToPixels(_width).Value; } }
        public float WidthTop { get { return Width.Top.ToPixels(_height).Value; } }
        public float WidthBottom { get { return Width.Bottom.ToPixels(_height).Value; } }

		#region IBorderStyle implementation

		public void RenderBorderBack(ISquare square)
		{
			runAnimation();

			if (!FillBackground) return;

			var slice = Slice.ToPercentage(_width, _height);
			var width = Width.ToPixels(_width, _height);
			var outset = Outset.ToPixels(_width, _height);

			float farLeft = square.TopLeft.X - outset.Left.Value;
			float farRight = square.TopRight.X + outset.Right.Value;
			float farTop = square.TopLeft.Y + outset.Top.Value;
			float farBottom = square.BottomLeft.Y - outset.Bottom.Value;

			var quad = new AGSSquare (new PointF (farLeft + width.Left.Value, farBottom + width.Bottom.Value),
				           new PointF (farRight - width.Right.Value, farBottom + width.Bottom.Value),
				           new PointF (farLeft + width.Left.Value, farTop - width.Top.Value), 
				           new PointF (farRight - width.Right.Value, farTop - width.Top.Value));

			drawQuad(quad, new FourCorners<Vector2> (new Vector2 (slice.Left.Value, slice.Bottom.Value),
				new Vector2 (1f - slice.Right.Value, slice.Bottom.Value), new Vector2 (slice.Left.Value, 1f - slice.Top.Value),
				new Vector2 (1f - slice.Right.Value, 1f - slice.Top.Value)));

			if (DrawBorderBehind) drawBorders(square);
		}

		public void RenderBorderFront(ISquare square)
		{
			if (!DrawBorderBehind) drawBorders(square);
		}

		#endregion

		private void drawBorders(ISquare square)
		{
			var slice = Slice.ToPercentage(_width, _height);
			var width = Width.ToPixels(_width, _height);
			var outset = Outset.ToPixels(_width, _height);

			float farLeft = square.TopLeft.X - outset.Left.Value;
			float farRight = square.TopRight.X + outset.Right.Value;
			float farTop = square.TopLeft.Y + outset.Top.Value;
			float farBottom = square.BottomLeft.Y - outset.Bottom.Value;
			SliceValues border = new SliceValues (SliceMeasurement.Pixels, farLeft, farRight, farTop, farBottom);
			drawCorners(border, slice, width);
			switch (Repeat)
			{
				case BorderRepeat.Repeat:
				case BorderRepeat.Round:
				case BorderRepeat.Space:
					drawEdges(border, slice, width, Repeat);
					break;
				case BorderRepeat.Stretch:
					drawStretch(border, slice, width);
					break;
				default:
					throw new NotSupportedException (Repeat.ToString());
			}
		}

		private void drawCorners(SliceValues border, SliceValues slice, SliceValues width)
		{
			var topLeftQuad = new AGSSquare(new PointF(border.Left.Value, border.Top.Value - width.Top.Value),
				new PointF(border.Left.Value + width.Top.Value, border.Top.Value - width.Top.Value),
				new PointF(border.Left.Value, border.Top.Value), new PointF(border.Left.Value + width.Top.Value, border.Top.Value));

			var topRightQuad = new AGSSquare(new PointF(border.Right.Value - width.Right.Value, border.Top.Value - width.Top.Value),
				new PointF(border.Right.Value, border.Top.Value - width.Top.Value),
				new PointF(border.Right.Value - width.Right.Value, border.Top.Value), new PointF(border.Right.Value, border.Top.Value));

			var bottomLeftQuad = new AGSSquare(new PointF(border.Left.Value, border.Bottom.Value),
				new PointF(border.Left.Value + width.Top.Value, border.Bottom.Value), new PointF(border.Left.Value, border.Bottom.Value + width.Bottom.Value), 
				new PointF(border.Left.Value + width.Top.Value, border.Bottom.Value + width.Bottom.Value));

			var bottomRightQuad = new AGSSquare(new PointF(border.Right.Value - width.Right.Value, border.Bottom.Value),
				new PointF(border.Right.Value, border.Bottom.Value), new PointF(border.Right.Value - width.Right.Value, border.Bottom.Value + width.Bottom.Value), 
				new PointF(border.Right.Value, border.Bottom.Value + width.Bottom.Value));

			drawQuad(topLeftQuad, new FourCorners<Vector2> (new Vector2(0f, 1f - slice.Top.Value), new Vector2(slice.Left.Value, 1f - slice.Top.Value), 
				new Vector2(0f, 1f), new Vector2(slice.Left.Value, 1f)));

			drawQuad(topRightQuad, new FourCorners<Vector2> (new Vector2(1f - slice.Right.Value, 1f - slice.Top.Value), new Vector2(1f, 1f - slice.Top.Value), 
				new Vector2(1f - slice.Right.Value, 1f), new Vector2(1f, 1f)));

			drawQuad(bottomLeftQuad, new FourCorners<Vector2> (new Vector2(0f, 0f), new Vector2(slice.Left.Value, 0f),
				new Vector2(0f, slice.Bottom.Value), new Vector2(slice.Left.Value, slice.Bottom.Value)));

			drawQuad(bottomRightQuad, new FourCorners<Vector2> (new Vector2(1f - slice.Right.Value, 0f), new Vector2(1f, 0f), 
				new Vector2(1f - slice.Right.Value, slice.Bottom.Value), new Vector2(1f, slice.Bottom.Value)));
		}

		private void drawQuad(ISquare quad, FourCorners<Vector2> texturePos)
		{
			GLUtils.DrawQuad(_texture, quad.BottomLeft.ToVector3(), quad.BottomRight.ToVector3(),
				quad.TopLeft.ToVector3(), quad.TopRight.ToVector3(), _white, texturePos); 
		}

		private void drawStretch(SliceValues border, SliceValues slice, SliceValues width)
		{
			var topQuad = new AGSSquare (new PointF (border.Left.Value + width.Left.Value, border.Top.Value - width.Top.Value),
				              new PointF (border.Right.Value - width.Right.Value, border.Top.Value - width.Top.Value), 
				              new PointF (border.Left.Value + width.Left.Value, border.Top.Value),
				              new PointF (border.Right.Value - width.Right.Value, border.Top.Value));


			var bottomQuad = new AGSSquare (new PointF (border.Left.Value + width.Left.Value, border.Bottom.Value),
				                 new PointF (border.Right.Value - width.Right.Value, border.Bottom.Value), 
				                 new PointF (border.Left.Value + width.Left.Value, border.Bottom.Value + width.Bottom.Value),
				                 new PointF (border.Right.Value - width.Right.Value, border.Bottom.Value + width.Bottom.Value));

			var leftQuad = new AGSSquare (new PointF (border.Left.Value, border.Bottom.Value + width.Bottom.Value),
				               new PointF (border.Left.Value + width.Left.Value, border.Bottom.Value + width.Bottom.Value), 
				               new PointF (border.Left.Value, border.Top.Value - width.Top.Value),
				               new PointF (border.Left.Value + width.Left.Value, border.Top.Value - width.Top.Value));

			var rightQuad = new AGSSquare (new PointF (border.Right.Value - width.Right.Value, border.Bottom.Value + width.Bottom.Value),
				                new PointF (border.Right.Value, border.Bottom.Value + width.Bottom.Value), 
				                new PointF (border.Right.Value - width.Right.Value, border.Top.Value - width.Top.Value),
				                new PointF (border.Right.Value, border.Top.Value - width.Top.Value));

			drawQuad(topQuad, new FourCorners<Vector2> (new Vector2 (slice.Left.Value, 1f - slice.Top.Value),
				new Vector2 (1f - slice.Right.Value, 1f - slice.Top.Value), new Vector2 (slice.Left.Value, 1f),
				new Vector2 (1f - slice.Right.Value, 1f)));

			drawQuad(bottomQuad, new FourCorners<Vector2> (new Vector2 (slice.Left.Value, 0f),
				new Vector2 (1f - slice.Right.Value, 0f), new Vector2 (slice.Left.Value, slice.Bottom.Value),
				new Vector2 (1f - slice.Right.Value, slice.Bottom.Value)));

			drawQuad(leftQuad, new FourCorners<Vector2> (new Vector2 (0f, slice.Bottom.Value),
				new Vector2 (slice.Left.Value, slice.Bottom.Value), new Vector2 (0f, 1f - slice.Top.Value),
				new Vector2 (slice.Left.Value, 1f - slice.Top.Value)));

			drawQuad(rightQuad, new FourCorners<Vector2> (new Vector2 (1f - slice.Right.Value, slice.Bottom.Value),
				new Vector2 (1f, slice.Bottom.Value), new Vector2 (1f - slice.Right.Value, 1f - slice.Top.Value),
				new Vector2 (1f, 1f - slice.Top.Value)));
		}
			
		private void drawEdges(SliceValues border, SliceValues slice, SliceValues width, 
			BorderRepeat repeat)
		{
			float leftX = border.Left.Value + width.Left.Value;
			float rightX = border.Right.Value - width.Right.Value;
			float rowWidth = rightX - leftX;
			float textureWorldWidth = ((1f - slice.Left.Value - slice.Right.Value) * _width);
			int stepsX = (int)(rowWidth / textureWorldWidth);
			float remainderX = rowWidth - textureWorldWidth * ((float)stepsX);

			float textureWidth = 1f - slice.Left.Value - slice.Right.Value;
			if (textureWidth < 0f) textureWidth = 1f;
			float topY = border.Top.Value - width.Top.Value;
			float farBottomY = border.Bottom.Value;
			float stepX = textureWorldWidth;
			float widthX = textureWorldWidth;
			float startX = leftX;
			if (remainderX > 0.001f)
			{
				switch (repeat)
				{
					case BorderRepeat.Round:
						stepX = rowWidth / stepsX;
						widthX = stepX;
						break;
					case BorderRepeat.Space:
						stepX = rowWidth / stepsX;
						startX = startX + (stepX - widthX) / 2f;
						break;
				}
			}
			drawHorizQuadRows(stepsX, startX, widthX, topY, farBottomY,
				width.Top.Value, width.Bottom.Value, textureWidth, 
				slice.Top.Value, slice.Bottom.Value, slice.Left.Value, 1f - slice.Top.Value, 0f,
				stepX);

			float bottomY = border.Bottom.Value + width.Bottom.Value;
			float rowHeight = topY - bottomY;
			float textureWorldHeight = ((1f - slice.Top.Value - slice.Bottom.Value) * _height);
			int stepsY = (int)(rowHeight / textureWorldHeight);
			float textureHeight = 1f - slice.Top.Value - slice.Bottom.Value;
			if (textureHeight < 0f) textureHeight = 1f;
			float remainderY = rowHeight - textureWorldHeight * ((float)stepsY);
			float stepY = textureWorldHeight;
			float heightY = textureWorldHeight;
			float startY = bottomY;
			if (remainderY > 0.001f)
			{
				switch (repeat)
				{
					case BorderRepeat.Round:
						stepY = rowHeight / stepsY;
						heightY = stepY;
						break;
					case BorderRepeat.Space:
						stepY = rowHeight / stepsY;
						startY = startY + (stepY - heightY) / 2f;
						break;
				}
			}
			drawVertQuadRows(stepsY, border.Left.Value, rightX, width.Left.Value, width.Right.Value,
				startY, heightY, slice.Left.Value, slice.Right.Value, textureHeight, 0f, 1f - slice.Right.Value,
				slice.Bottom.Value, stepY);

			if (repeat == BorderRepeat.Repeat)
			{
				if (remainderX > 0.0001f)
				{
					textureWidth = (remainderX / textureWorldWidth) * textureWidth;
					leftX = leftX + textureWorldWidth * stepsX;
							drawHorizQuadRows(1, leftX, remainderX, topY, farBottomY, width.Top.Value, width.Bottom.Value, textureWidth,
						slice.Top.Value, slice.Bottom.Value, slice.Left.Value, 1f - slice.Top.Value, 0f, remainderX);
				}

				if (remainderY > 0.001f)
				{
					textureHeight = (remainderY / textureWorldHeight) * textureHeight;
					bottomY = bottomY + textureWorldHeight * stepsY;
					drawVertQuadRows(1, border.Left.Value, rightX, width.Left.Value, width.Right.Value, bottomY, remainderY,
						slice.Left.Value, slice.Right.Value, textureHeight, 0f, 1f - slice.Right.Value, slice.Bottom.Value, remainderY);
				}
			}
		}

		private void drawHorizQuadRows(int steps, float x, float width, float y1, float y2, float height1, float height2, 
			float textureWidth, float textureHeight1, float textureHeight2, float textureX, float textureY1, float textureY2, float stepX)
		{
			drawQuadRow(steps, x, width, y1, height1, textureWidth, textureHeight1, textureX, textureY1, stepX, 0f);
			drawQuadRow(steps, x, width, y2, height2, textureWidth, textureHeight2, textureX, textureY2, stepX, 0f);
		}

		private void drawVertQuadRows(int steps, float x1, float x2, float width1, float width2, float y, float height, 
			float textureWidth1, float textureWidth2, float textureHeight, float textureX1, float textureX2, float textureY, float stepY)
		{
			drawQuadRow(steps, x1, width1, y, height, textureWidth1, textureHeight, textureX1, textureY, 0f, stepY);
			drawQuadRow(steps, x2, width2, y, height, textureWidth2, textureHeight, textureX2, textureY, 0f, stepY);
		}

		private void drawQuadRow(int steps, float x, float width, float y, float height, float textureWidth, float textureHeight,
			float textureX, float textureY, float stepX, float stepY)
		{
			for (int i = 0; i < steps; i++)
			{
				var quad = new AGSSquare (new PointF (x, y), new PointF (x + width, y), new PointF (x, y + height), new PointF (x + width, y + height));
				drawQuad(quad, new FourCorners<Vector2> (new Vector2 (textureX, textureY), new Vector2 (textureX + textureWidth, textureY),
					new Vector2 (textureX, textureY + textureHeight), new Vector2 (textureX + textureWidth, textureY + textureHeight)));
				x += stepX;
				y += stepY;
			}
		}

		private void runAnimation()
		{
			if (Image == null || Image.State.IsPaused) return;
			Image.State.TimeToNextFrame--;
			if (Image.State.TimeToNextFrame < 0)
				Image.NextFrame();

			var frame = Image.Frames[Image.State.CurrentFrame];
			if (frame.Sprite == null) return;
			GLImage image = frame.Sprite.Image as GLImage;
			if (image == null) return;
			_texture = image.Texture;
			_width = image.Width;
			_height = image.Height;
		}
	}
}

