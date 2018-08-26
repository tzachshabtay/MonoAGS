using System;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
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
            PlayerState lastState = _lastState;
            _lastState = currentState;
            if (lastState.Character == character)
			{                
				checkHorizontalEdges(currentState, lastState);
				checkVerticalEdges(currentState, lastState);
			}			
		}

		public void FromEdges(IEdges edges)
		{
			Left = edges.Left;
			Right = edges.Right;
			Top = edges.Top;
			Bottom = edges.Bottom;
		}

        public override string ToString() => $"({Left}-{Right}), ({Bottom}-{Top})";

        private void checkHorizontalEdges(PlayerState currentState, PlayerState lastState)
		{
			float previous = lastState.X;
			float current = currentState.X;
			if (!checkEdgeCross(Left, previous, current, crossedEdgeDownwards))
				checkEdgeCross(Right, previous, current, crossedEdgeUpwards);
		}

		private void checkVerticalEdges(PlayerState currentState, PlayerState lastState)
		{
			float previous = lastState.Y;
			float current = currentState.Y;
			if (!checkEdgeCross(Bottom, previous, current, crossedEdgeDownwards))
				checkEdgeCross(Top, previous, current, crossedEdgeUpwards);
		}

		private bool checkEdgeCross(IEdge edge, float previous, float current, Func<IEdge,float,float,bool> check)
		{
            if (!edge.Enabled) return false;
			if (!check(edge,previous,current)) return false;
			edge.OnEdgeCrossed.Invoke();
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
			public PlayerState(ICharacter character, float x, float y) : this()
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

