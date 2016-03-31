using System;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public partial class AGSCharacter
	{
		partial void init(Resolver resolver, IOutfit outfit)
		{
			RenderLayer = AGSLayers.Foreground;
			IgnoreScalingArea = false;

			_hasOutfit.Outfit = outfit;

			TypedParameter objParameter = new TypedParameter (typeof(IObject), this);
			_faceDirectionBehavior.CurrentDirectionalAnimation = outfit.IdleAnimation;
			TypedParameter outfitParameter = new TypedParameter (typeof(IHasOutfit), this);
			ISayLocation location = resolver.Container.Resolve<ISayLocation>(objParameter);
			TypedParameter locationParameter = new TypedParameter (typeof(ISayLocation), location);
			TypedParameter faceDirectionParameter = new TypedParameter (typeof(IFaceDirectionBehavior), _faceDirectionBehavior);
			_sayBehavior = resolver.Container.Resolve<ISayBehavior>(locationParameter, outfitParameter, faceDirectionParameter);
			_walkBehavior = resolver.Container.Resolve<IWalkBehavior>(objParameter, outfitParameter, faceDirectionParameter);
			AddComponent(_sayBehavior);
			AddComponent(_walkBehavior);
		}

		public ILocation Location
		{
			get { return _animationContainer.Location; } 
			set
			{
				StopWalking();
				_animationContainer.Location = value; 
			}
		}

		public void ChangeRoom(IRoom room, float? x = null, float? y = null)
		{
			StopWalking();
			_hasRoom.ChangeRoom(room, x, y);
		}
	}
}

