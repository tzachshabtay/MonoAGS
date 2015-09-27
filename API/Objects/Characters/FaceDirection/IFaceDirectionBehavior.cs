using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public enum Direction
	{
		Down,
		Up,
		Left,
		Right,
		DownLeft,
		DownRight,
		UpLeft,
		UpRight,
	}

	public interface IFaceDirectionBehavior
	{
		Direction Direction { get; }
		IDirectionalAnimation CurrentDirectionalAnimation { get; }

		void FaceDirection(Direction direction);
		Task FaceDirectionAsync(Direction direction);

		void FaceDirection(IObject obj);
		Task FaceDirectionAsync(IObject obj);

		void FaceDirection(float x, float y);
		Task FaceDirectionAsync(float x, float y);

		void FaceDirection(float fromX, float fromY, float toX, float toY);
		Task FaceDirectionAsync(float fromX, float fromY, float toX, float toY);
	}
}

