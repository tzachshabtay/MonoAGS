using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSCollider : ICollider
	{
		private IGameState _state;
		private IObject _obj;

		public AGSCollider(IObject obj, IGameState state)
		{
			_obj = obj;
			_state = state;
		}

		public ISquare BoundingBox { get; set; }

		public IPoint CenterPoint
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

				return new AGSPoint (minX + offsetX, minY + offsetY);
			}
		}

		public bool CollidesWith(float x, float y)
		{
			
			ISquare boundingBox = BoundingBox;
			if (boundingBox == null)
				return false;
			IArea pixelPerfect = _obj.PixelPerfectHitTestArea;

			if (_obj.IgnoreViewport && _state != null) //todo: account for viewport scaling as well
			{
				x -= _state.Player.Character.Room.Viewport.X;
				y -= _state.Player.Character.Room.Viewport.Y;
			}

			if (pixelPerfect == null || !pixelPerfect.Enabled)
			{
				if (boundingBox.Contains(new AGSPoint (x, y)))
					return true;
			}
			else
			{
				if (pixelPerfect.IsInArea(new AGSPoint (x, y), boundingBox, _obj.ScaleX * _obj.Animation.Sprite.ScaleX,
					_obj.ScaleY * _obj.Animation.Sprite.ScaleY))
					return true;
			}
			return false;
		}
	}
}

