using System.Linq;
using AGS.API;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractViewport : IContract<IViewport>
	{
		public ContractViewport()
		{
		}

		[ProtoMember(1)]
		public float X { get; set; }

		[ProtoMember(2)]
		public float Y { get; set; }
		 
		[ProtoMember(3)]
		public float ScaleX { get; set; }

		[ProtoMember(4)]
		public float ScaleY { get; set; }
		 
		[ProtoMember(5)]
		public Contract<ICamera> Camera { get; set; }

        [ProtoMember(6)]
        public IContract<RectangleF> ProjectionBox { get; set; }

        [ProtoMember(7)]
        public IContract<IDisplayListSettings> DisplayListSettings { get; set; }

        [ProtoMember(8)]
        public bool IsRoomProviderGameState { get; set; }

        [ProtoMember(9)]
        public string RoomProviderRoomID { get; set; }

		#region IContract implementation

		public IViewport ToItem(AGSSerializationContext context)
		{
            AGSViewport viewport = new AGSViewport(DisplayListSettings.ToItem(context) ,Camera.ToItem(context));
			viewport.X = X;
			viewport.Y = Y;
			viewport.ScaleX = ScaleX;
			viewport.ScaleY = ScaleY;
            viewport.ProjectionBox = ProjectionBox.ToItem(context);

            context.Rewire(state => 
            {
                if (IsRoomProviderGameState) viewport.RoomProvider = state;
                else 
                {
                    var room = state.Rooms.FirstOrDefault(r => r.ID == RoomProviderRoomID);
                    if (room != null) viewport.RoomProvider = new AGSSingleRoomProvider(room);
                }
            });
		
			return viewport;
		}

		public void FromItem(AGSSerializationContext context, IViewport item)
		{
			Camera = new Contract<ICamera> ();
			Camera.FromItem(context, item.Camera);

			X = item.X;
			Y = item.Y;
			ScaleX = item.ScaleX;
			ScaleY = item.ScaleY;
            ProjectionBox = context.GetContract(item.ProjectionBox);
            DisplayListSettings = context.GetContract(item.DisplayListSettings);
            IsRoomProviderGameState = item.RoomProvider == context.Resolver.Container.Resolve<IGameState>();
            if (!IsRoomProviderGameState)
            {
                RoomProviderRoomID = item.RoomProvider.Room.ID;
            }
		}

		#endregion
	}
}

