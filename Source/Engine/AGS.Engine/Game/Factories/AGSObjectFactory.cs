using System.Diagnostics;
using System.Threading.Tasks;
using AGS.API;

using Autofac;

namespace AGS.Engine
{
	public class AGSObjectFactory : IObjectFactory
	{
        private readonly Resolver _resolver;
		private readonly IGameState _gameState;
        private IMaskLoader _maskLoader; //We can't get mask loader in the constructor due to a circular dependency -> mask loader requires the game factory, and the game factory requires the object factory

        public AGSObjectFactory(Resolver resolver, IGameState gameState)
		{
			_resolver = resolver;
			_gameState = gameState;
		}

		public IObject GetObject(string id, IRoom room = null)
		{
            Debug.WriteLine("Getting object: " + id ?? "null");
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			var obj = _resolver.Container.Resolve<IObject>(idParam);
            room?.Objects.Add(obj);
            return obj;
		}

        public IObject GetAdventureObject(string id, IRoom room = null, string[] sayWhenLook = null, string[] sayWhenInteract = null)
        {
            IObject obj = GetObject(id);
            IHotspotComponent hotspot = obj.AddComponent<IHotspotComponent>();
            subscribeSentences(sayWhenLook, hotspot.Interactions.OnInteract(AGSInteractions.LOOK));
            subscribeSentences(sayWhenInteract, hotspot.Interactions.OnInteract(AGSInteractions.INTERACT));
            room?.Objects.Add(obj);
            return obj;
        }

        public ICharacter GetCharacter(string id, IOutfit outfit, IRoom room = null, string[] sayWhenLook = null, string[] sayWhenInteract = null)
		{
			ICharacter character = GetCharacter(id, outfit, _resolver.Container.Resolve<IAnimationComponent>());

            subscribeSentences(sayWhenLook, character.Interactions.OnInteract(AGSInteractions.LOOK));
            subscribeSentences(sayWhenInteract, character.Interactions.OnInteract(AGSInteractions.INTERACT));

            room?.Objects.Add(character);
			return character;
		}

		public ICharacter GetCharacter(string id, IOutfit outfit, IAnimationComponent container, IRoom room = null)
		{
			TypedParameter outfitParam = new TypedParameter (typeof(IOutfit), outfit);
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			TypedParameter animationParam = new TypedParameter (typeof(IAnimationComponent), container);
			ICharacter character = _resolver.Container.Resolve<ICharacter>(outfitParam, idParam, animationParam);
            room?.Objects.Add(character);
			return character;
		}

		public IObject GetHotspot(string maskPath, string hotspot, IRoom room = null, string[] sayWhenLook = null, 
			string[] sayWhenInteract = null, string id = null)
		{
            _maskLoader = _maskLoader ?? _resolver.Container.Resolve<IMaskLoader>();
			IMask mask = _maskLoader.Load(maskPath, debugDrawColor:  Colors.White, id: id ?? hotspot);
            if (mask == null) return newAdventureObject(id ?? hotspot);
			setMask (mask, hotspot, room, sayWhenLook, sayWhenInteract);
			return mask.DebugDraw;
		}

		public async Task<IObject> GetHotspotAsync(string maskPath, string hotspot, IRoom room = null, string[] sayWhenLook = null,
			string[] sayWhenInteract = null, string id = null)
		{
            id = id ?? $"{hotspot} {maskPath}";
            _maskLoader = _maskLoader ?? _resolver.Container.Resolve<IMaskLoader>();
			IMask mask = await _maskLoader.LoadAsync(maskPath, debugDrawColor: Colors.White, id: id);
            if (mask == null) return newAdventureObject(id);
			setMask (mask, hotspot, room, sayWhenLook, sayWhenInteract);
			return mask.DebugDraw;
		}

        private IObject newAdventureObject(string id)
        {
            IObject obj = new AGSObject(id, _resolver);
            obj.AddComponent<IHotspotComponent>();
            return obj;
        }

		private void setMask(IMask mask, string hotspot, IRoom room, string [] sayWhenLook, string [] sayWhenInteract)
		{
            mask.DebugDraw.IsPixelPerfect = true;
            mask.DebugDraw.Enabled = true;
			mask.DebugDraw.DisplayName = hotspot;
			mask.DebugDraw.Opacity = 0;
			mask.DebugDraw.Z = mask.MinY;

            IHotspotComponent hotobj = mask.DebugDraw.GetComponent<IHotspotComponent>();
            subscribeSentences (sayWhenLook, hotobj.Interactions.OnInteract(AGSInteractions.LOOK));
            subscribeSentences (sayWhenInteract, hotobj.Interactions.OnInteract(AGSInteractions.INTERACT));
            room?.Objects.Add(mask.DebugDraw);
		}

		private void subscribeSentences(string[] sentences, IEvent<ObjectEventArgs> e)
		{
			if (sentences == null || e == null) return;

			e.SubscribeToAsync(async (_) =>
			{
				foreach (string sentence in sentences)
				{
					await _gameState.Player.SayAsync(sentence);
				}
			});
		}
	}
}