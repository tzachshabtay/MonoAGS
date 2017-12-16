using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public partial class AGSCharacter
	{
        partial void beforeInitComponents(Resolver resolver, IOutfit outfit)
        {
            TypedParameter objParameter = new TypedParameter(typeof(IObject), this);
            if (outfit != null) _faceDirectionComponent.CurrentDirectionalAnimation = outfit[AGSOutfit.Idle];
            ISayLocationProvider location = resolver.Container.Resolve<ISayLocationProvider>(objParameter);
            TypedParameter locationParameter = new TypedParameter(typeof(ISayLocationProvider), location);
            _sayComponent = resolver.Container.Resolve<ISayComponent>(locationParameter);

            //todo: refactor ISayLocationProvider into a component, then bind to it from say behavior, then add walk & say to the template and remove from here
            _walkComponent = AddComponent<IWalkComponent>();
            AddComponent<ISayComponent>(_sayComponent);
        }

        partial void afterInitComponents(Resolver resolver, IOutfit outfit)
		{
			RenderLayer = AGSLayers.Foreground;
			IgnoreScalingArea = false;

			_hasOutfit.Outfit = outfit;
		}

		public async Task ChangeRoomAsync(IRoom room, float? x = null, float? y = null)
		{
			await StopWalkingAsync();
			await _hasRoom.ChangeRoomAsync(room, x, y);
		}
	}
}

