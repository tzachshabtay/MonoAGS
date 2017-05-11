using System;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSRoomFactory : IRoomFactory
	{
        private readonly Resolver _resolver;

		public AGSRoomFactory(Resolver resolver)
		{
			_resolver = resolver;
		}

		public IEdge GetEdge(float value = 0f)
		{
			IEdge edge = _resolver.Container.Resolve<IEdge>();
			edge.Value = value;
			return edge;
		}

		public IRoom GetRoom(string id, float leftEdge = 0f, float rightEdge = 0f, float topEdge = 0f, float bottomEdge = 0f)
		{
			AGSEdges edges = new AGSEdges (GetEdge(leftEdge), GetEdge(rightEdge), GetEdge(topEdge), GetEdge(bottomEdge));
			TypedParameter edgeParam = new TypedParameter (typeof(IAGSEdges), edges);
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			IRoom room = _resolver.Container.Resolve<IRoom>(idParam, edgeParam);
			room.Viewport.Camera = _resolver.Container.Resolve<ICamera>();
            IGameState state = _resolver.Container.Resolve<IGameState>();
            room.Viewport.Camera.Target = () => state.Player;
			return room;
		}
	}
}

