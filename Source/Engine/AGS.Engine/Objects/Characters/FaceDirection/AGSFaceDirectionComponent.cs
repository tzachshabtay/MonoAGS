﻿using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSFaceDirectionComponent : AGSComponent, IFaceDirectionComponent
	{
		private IAnimationComponent _obj;
        private ITranslate _translate;

		public override void Init()
		{
			base.Init();
            Entity.Bind<IAnimationComponent>(c => _obj = c, _ => _obj = null);
            Entity.Bind<ITranslateComponent>(c => _translate = c, _ => _translate = null);
		}

		#region IFaceDirectionBehavior implementation

		public Direction Direction { get; private set; }
		public IDirectionalAnimation CurrentDirectionalAnimation { get; set; }

		public async Task FaceDirectionAsync(Direction direction)
		{
			Direction = direction;
			IDirectionalAnimation animation = CurrentDirectionalAnimation;
			if (animation == null) return;

			switch (direction)
			{
				case Direction.Down:
					await faceDirectionAsync(Direction.Down, Direction.Up, Direction.Left, Direction.Right);
					return;
				case Direction.Up:
					await faceDirectionAsync(Direction.Up, Direction.Down, Direction.Left, Direction.Right);
					return;
				case Direction.Left:
					await faceDirectionAsync(Direction.Left, Direction.Down, Direction.Up, Direction.Right);
					return;
				case Direction.Right:
					await faceDirectionAsync(Direction.Right, Direction.Down, Direction.Up, Direction.Left);
					return;
				case Direction.DownRight:
					await faceDirectionAsync(Direction.DownRight, Direction.Right, Direction.Down, Direction.Up, Direction.Left);
					return;
				case Direction.DownLeft:
					await faceDirectionAsync(Direction.DownLeft, Direction.Left, Direction.Down, Direction.Up, Direction.Right);
					return;
				case Direction.UpRight:
					await faceDirectionAsync(Direction.UpRight, Direction.Right, Direction.Up, Direction.Down, Direction.Left);
					return;
				case Direction.UpLeft:
					await faceDirectionAsync(Direction.UpLeft, Direction.Left, Direction.Up, Direction.Down, Direction.Right);
					return;
				default:
					throw new NotSupportedException ("Direction is not supported: " + direction.ToString());
			}
		}
			
		public async Task FaceDirectionAsync(IObject obj)
		{
            PointF? point = obj.CenterPoint ?? obj.Position.XY;
			await FaceDirectionAsync(point.Value.X, point.Value.Y);
		}

		public async Task FaceDirectionAsync(float x, float y)
		{
			var translate = _translate;
			if (translate == null) return;
			await FaceDirectionAsync(translate.X, translate.Y, x, y);
		}

		public async Task FaceDirectionAsync(float fromX, float fromY, float toX, float toY)
		{
			await setDirection(fromX, fromY, toX, toY);
		}

		#endregion

		private async Task setDirection(float xSource, float ySource, float xDest, float yDest)
		{
			float angle = getAngle (xSource, ySource, xDest, yDest);
			IDirectionalAnimation animation = CurrentDirectionalAnimation;
			if (animation == null) return;

			if (angle < -30 && angle > -60 && animation.DownRight != null)
			{
				Direction = Direction.DownRight;
				await changeAnimationIfNeeded(animation.DownRight);
			}
			else if (angle < -120 && angle > -150 && animation.DownLeft != null)
			{
				Direction = Direction.DownLeft;
				await changeAnimationIfNeeded(animation.DownLeft);
			}
			else if (angle > 120 && angle < 150 && animation.UpLeft != null)
			{
				Direction = Direction.UpLeft;
				await changeAnimationIfNeeded(animation.UpLeft);
			}
			else if (angle > 30 && angle < 60 && animation.UpRight != null)
			{
				Direction = Direction.UpRight;
				await changeAnimationIfNeeded(animation.UpRight);
			}
			else if (angle < -65 && angle > -115 && animation.Down != null)
			{
				Direction = Direction.Down;
				await changeAnimationIfNeeded(animation.Down);
			}
			else if (angle > 65 && angle < 115 && animation.Up != null)
			{
				Direction = Direction.Up;
				await changeAnimationIfNeeded(animation.Up);
			}
			else if (xDest > xSource && animation.Right != null)
			{
				Direction = Direction.Right;
				await changeAnimationIfNeeded(animation.Right);
			}
			else if (animation.Left != null)
			{
				Direction = Direction.Left;
				await changeAnimationIfNeeded(animation.Left);
			}
		}

		private async Task faceDirectionAsync(params Direction[] directions)
		{
			IDirectionalAnimation dirAnimation = CurrentDirectionalAnimation;
			if (dirAnimation == null) return;

			foreach (Direction dir in directions)
			{
                var animation = dirAnimation.GetAnimation(dir);
                if (await animateAsync(animation)) return;
			}
		}

		private async Task<bool> animateAsync(IAnimation animation)
		{
			if (animation == null) return false;
			await changeAnimationIfNeeded(animation);
			return true;
		}

		private async Task changeAnimationIfNeeded(IAnimation animation)
		{
            var obj = _obj;
            if (obj == null) return;
			if (animation == obj.Animation)
				return;
			await Task.Delay (1);
			obj.StartAnimation (animation);
			await Task.Delay (1);
		}

		private float getAngle(float x1, float y1, float x2, float y2)
		{
			float deltaX = x2 - x1;
			if (MathUtils.FloatEquals(deltaX, 0f))
				deltaX = 0.001f;
			float deltaY = y2 - y1;
			float angle = ((float)Math.Atan2 (deltaY, deltaX)) * 180f / (float)Math.PI;
			return angle;
		}
	}
}
