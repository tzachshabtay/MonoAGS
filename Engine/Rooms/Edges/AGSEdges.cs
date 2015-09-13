using System;
using API;

namespace Engine
{
	public class AGSEdges : IAGSEdges
	{
		private PlayerState _lastState;

		public AGSEdges(IEdge left, IEdge right, IEdge top, IEdge bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		#region IEdges implementation

		public IEdge Left { get; private set; }

		public IEdge Right { get; private set; }

		public IEdge Top { get; private set; }

		public IEdge Bottom { get; private set; }

		#endregion

		public void OnRepeatedlyExecute(ICharacter character)
		{
			PlayerState currentState = new PlayerState (character, character.X, character.Y);
			if (_lastState.Character == character)
			{
				checkHorizontalEdges(currentState);
				checkVerticalEdges(currentState);
			}
			_lastState = currentState;
		}

		private void checkHorizontalEdges(PlayerState currentState)
		{
			float previous = _lastState.X;
			float current = currentState.X;
			if (!checkEdgeCross(Left, previous, current, crossedEdgeDownwards))
				checkEdgeCross(Right, previous, current, crossedEdgeUpwards);
		}

		private void checkVerticalEdges(PlayerState currentState)
		{
			float previous = _lastState.Y;
			float current = currentState.Y;
			if (!checkEdgeCross(Bottom, previous, current, crossedEdgeDownwards))
				checkEdgeCross(Top, previous, current, crossedEdgeUpwards);
		}

		private bool checkEdgeCross(IEdge edge, float previous, float current, Func<IEdge,float,float,bool> check)
		{
			if (!check(edge,previous,current)) return false;
			edge.OnEdgeCrossed.Invoke(edge, new EventArgs());
			return true;
		}

		private bool crossedEdgeDownwards(IEdge edge, float previous, float current)
		{
			return edge.Value <= previous && edge.Value > current;
		}

		private bool crossedEdgeUpwards(IEdge edge, float previous, float current)
		{
			return edge.Value >= previous && edge.Value < current;
		}

		private struct PlayerState
		{
			public PlayerState(ICharacter character, float x, float y)
			{
				Character = character;
				X = x;
				Y = y;
			}

			public ICharacter Character { get; private set; }
			public float X { get; private set; }
			public float Y { get; private set; }
		}
	}
}

