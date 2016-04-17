using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSCollider : AGSComponent, ICollider
	{
		private IGameState _state;
		private IDrawableInfo _drawableInfo;
		private IAnimationContainer _obj;

		public AGSCollider(IGameState state)
		{
			_state = state;
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			_drawableInfo = entity.GetComponent<IDrawableInfo>();
			_obj = entity.GetComponent<IAnimationContainer>();
		}

		public ISquare BoundingBox { get; set; }

		public PointF? CenterPoint
		{
			get
			{
				if (BoundingBox == null) return null;
				float minX = BoundingBox.MinX;
				float minY = BoundingBox.MinY;
				float offsetX = _obj.PixelPerfectHitTestArea == null ? (BoundingBox.MaxX - BoundingBox.MinX) / 2f : 
					_obj.PixelPerfectHitTestArea.Mask.MinX + (_obj.PixelPerfectHitTestArea.Mask.MaxX - _obj.PixelPerfectHitTestArea.Mask.MinX) / 2f;
				float offsetY = _obj.PixelPerfectHitTestArea == null ? (BoundingBox.MaxY - BoundingBox.MinY) / 2f : 
					_obj.PixelPerfectHitTestArea.Mask.MinY + (_obj.PixelPerfectHitTestArea.Mask.MaxY - _obj.PixelPerfectHitTestArea.Mask.MinY) / 2f;

				return new PointF (minX + offsetX, minY + offsetY);
			}
		}

		public bool CollidesWith(float x, float y)
		{
			ISquare boundingBox = BoundingBox;
			if (boundingBox == null)
				return false;
			IArea pixelPerfect = _obj.PixelPerfectHitTestArea;

			if (_drawableInfo.IgnoreViewport && _state != null) //todo: account for viewport scaling as well
			{
				x -= _state.Player.Character.Room.Viewport.X;
				y -= _state.Player.Character.Room.Viewport.Y;
			}

			if (pixelPerfect == null || !pixelPerfect.Enabled)
			{
				if (boundingBox.Contains(new PointF (x, y)))
					return true;
			}
			else
			{
				if (pixelPerfect.IsInArea(new PointF (x, y), boundingBox, _obj.ScaleX * _obj.Animation.Sprite.ScaleX,
					_obj.ScaleY * _obj.Animation.Sprite.ScaleY))
					return true;
			}
			return false;
		}
	}
}

