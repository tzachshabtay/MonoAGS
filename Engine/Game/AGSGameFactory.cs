using System;
using AGS.API;
using Autofac;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSGameFactory : IGameFactory
	{
		private IContainer _resolver;
		private IGameState _gameState;

		public AGSGameFactory(IGraphicsFactory graphics, IGameState state, IContainer resolver)
		{
			Graphics = graphics;
			_resolver = resolver;
			_gameState = state;
		}

		#region IGameFactory implementation

		public int GetInt(string name, int defaultValue = 0)
		{
			throw new NotImplementedException();
		}

		public float GetFloat(string name, float defaultValue = 0f)
		{
			throw new NotImplementedException();
		}

		public string GetString(string name, string defaultValue = null)
		{
			throw new NotImplementedException();
		}

		public ILabel GetLabel(string text, float width, float height, float x, float y)
		{
			ILabel label = _resolver.Resolve<ILabel>();
			label.Text = text;
			label.ScaleTo(width, height);
			label.X = x;
			label.Y = y;
			label.Tint = Color.Transparent;
			_gameState.UI.Add(label);
			return label;
		}

		public IObject GetObject()
		{
			IObject obj = _resolver.Resolve<IObject>();
			return obj;
		}

		public ICharacter GetCharacter()
		{
			ICharacter character = _resolver.Resolve<ICharacter>();
			return character;
		}

		public void RegisterCustomData(ICustomSerializable customData)
		{
			throw new NotImplementedException();
		}

		public IGraphicsFactory Graphics { get; private set; }

		public ISoundFactory Sound
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}

