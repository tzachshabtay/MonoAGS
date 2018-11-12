using ProtoBuf;
using AGS.API;
using System.Collections.Generic;
using Autofac;

namespace AGS.Engine
{
	[ProtoContract(AsReferenceDefault = true)]
	public class ContractGameState : IContract<IGameState>
	{
	    [ProtoMember(1)]
        public ContractCharacter Player { get; set; }

		[ProtoMember(2)]
		public IList<IContract<IRoom>> Rooms { get; set; }

		[ProtoMember(3, AsReference = true)]
		public IList<IContract<IObject>> UI { get; set; }

		[ProtoMember(4)]
		public IContract<ICustomProperties> GlobalVariables { get; set; }

		[ProtoMember(5)]
		public IContract<ICutscene> Cutscene { get; set; }

        [ProtoMember(6)]
        public Dictionary<string, int> RepeatCounters { get; set; }

		[ProtoMember(7)]
		public int GameSpeed { get; set; }

		[ProtoMember(8)]
		public IContract<IViewport> Viewport { get; set; }

		[ProtoMember(9)]
		public IList<IContract<IViewport>> SecondaryViewports { get; set; }

		#region IContract implementation

		public IGameState ToItem(AGSSerializationContext context)
		{
            TypedParameter viewParam = new TypedParameter(typeof(IViewport), Viewport.ToItem(context));
			IGameState state = context.Resolver.Container.Resolve<IGameState>(viewParam);

            state.SecondaryViewports.Clear();
            if (SecondaryViewports != null)
            {
                foreach (var viewport in SecondaryViewports)
                {
                    state.SecondaryViewports.Add(viewport.ToItem(context));
                }
            }

			state.Rooms.Clear();
			if (Rooms != null)
			{
				foreach (var room in Rooms)
				{
					state.Rooms.Add(room.ToItem(context));
				}
			}

			state.UI.Clear();
			if (UI != null)
			{
				foreach (var ui in UI)
				{

					state.UI.Add(ui.ToItem(context));
				}
			}
			state.GlobalVariables.CopyFrom(GlobalVariables.ToItem(context));
			state.Cutscene.CopyFrom(Cutscene.ToItem(context));

            ICharacter player = Player.ToItem(context);
			state.Player = player;
			state.Speed = GameSpeed;
            
			if (RepeatCounters == null) RepeatCounters = new Dictionary<string, int> ();
            Repeat.FromDictionary(RepeatCounters);

			return state;
		}

		public void FromItem(AGSSerializationContext context, IGameState item)
		{
			UI = new List<IContract<IObject>> (item.UI.Count);
			foreach (var ui in item.UI)
			{
				var contract = context.GetContract(ui);
				if (contract == null) continue;
				UI.Add(contract);
			}
				
			Rooms = new List<IContract<IRoom>> (item.Rooms.Count);
			foreach (var room in item.Rooms)
			{
				Rooms.Add(context.GetContract(room));
			}

            Player = new ContractCharacter();
            Player.FromItem(context, item.Player);

			GlobalVariables = context.GetContract(item.GlobalVariables);
			Cutscene = context.GetContract(item.Cutscene);
            RepeatCounters = Repeat.ToDictionary();
			GameSpeed = item.Speed;
            Viewport = context.GetContract(item.Viewport);
            SecondaryViewports = new List<IContract<IViewport>>(item.SecondaryViewports.Count);
            foreach (var viewport in item.SecondaryViewports)
            {
                SecondaryViewports.Add(context.GetContract(viewport));
            }
		}

		#endregion
	}
}

