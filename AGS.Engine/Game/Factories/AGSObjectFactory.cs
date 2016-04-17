using System;
using AGS.API;

using Autofac;

namespace AGS.Engine
{
	public class AGSObjectFactory : IObjectFactory
	{
		private IContainer _resolver;
		private IGameState _gameState;

		public AGSObjectFactory(IContainer resolver, IGameState gameState)
		{
			_resolver = resolver;
			_gameState = gameState;
		}

		public IObject GetObject(string id, string[] sayWhenLook = null, string[] sayWhenInteract = null)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			IObject obj = _resolver.Resolve<IObject>(idParam);

			subscribeSentences(sayWhenLook, obj.Interactions.OnLook);
			subscribeSentences(sayWhenInteract, obj.Interactions.OnInteract);

			return obj;
		}

		public ICharacter GetCharacter(string id, IOutfit outfit, string[] sayWhenLook = null, string[] sayWhenInteract = null)
		{
			ICharacter character = GetCharacter(id, outfit, _resolver.Resolve<IAnimationContainer>());

			subscribeSentences(sayWhenLook, character.Interactions.OnLook);
			subscribeSentences(sayWhenInteract, character.Interactions.OnInteract);

			return character;
		}

		public ICharacter GetCharacter(string id, IOutfit outfit, IAnimationContainer container)
		{
			TypedParameter outfitParam = new TypedParameter (typeof(IOutfit), outfit);
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			TypedParameter animationParam = new TypedParameter (typeof(IAnimationContainer), container);
			ICharacter character = _resolver.Resolve<ICharacter>(outfitParam, idParam, animationParam);
			return character;
		}

		public IObject GetHotspot(string maskPath, string hotspot, string[] sayWhenLook = null, 
			string[] sayWhenInteract = null, string id = null)
		{
			IMaskLoader maskLoader = _resolver.Resolve<IMaskLoader>();
			IMask mask = maskLoader.Load(maskPath, debugDrawColor:  Colors.White, id: id ?? hotspot);
			mask.DebugDraw.PixelPerfect(true);
			mask.DebugDraw.Hotspot = hotspot;
			mask.DebugDraw.Opacity = 0;
			mask.DebugDraw.Z = mask.MinY;

			subscribeSentences(sayWhenLook, mask.DebugDraw.Interactions.OnLook);
			subscribeSentences(sayWhenInteract, mask.DebugDraw.Interactions.OnInteract);

			return mask.DebugDraw;
		}

		private void subscribeSentences(string[] sentences, IEvent<ObjectEventArgs> e)
		{
			if (sentences == null || e == null) return;

			e.SubscribeToAsync(async (sender, args) =>
			{
				foreach (string sentence in sentences)
				{
					await _gameState.Player.Character.SayAsync(sentence);
				}
			});
		}
	}
}

