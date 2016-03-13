using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;
using Autofac;

namespace AGS.Engine
{
	[ProtoContract(AsReferenceDefault = true)]
	public class ContractGameState : IContract<IGameState>
	{
		public ContractGameState()
		{
		}

		[ProtoMember(1)]
		public IContract<IPlayer> Player { get; set; }

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
		#region IContract implementation

		public IGameState ToItem(AGSSerializationContext context)
		{
			IGameState state = context.Resolver.Container.Resolve<IGameState>();

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

			IPlayer player = Player.ToItem(context);
			var updater = new ContainerBuilder ();
			updater.RegisterInstance(player).As<IPlayer>();
			updater.Update(context.Resolver.Container);
			state.Player = player;
            
            Repeat.FromDictionary(RepeatCounters);

			return state;
		}

		public void FromItem(AGSSerializationContext context, IGameState item)
		{
			UI = new List<IContract<IObject>> (item.UI.Count);
			foreach (var ui in item.UI)
			{
				UI.Add(context.GetContract(ui));
			}
				
			Rooms = new List<IContract<IRoom>> (item.Rooms.Count);
			foreach (var room in item.Rooms)
			{
				Rooms.Add(context.GetContract(room));
			}

			Player = context.GetContract(item.Player);

			GlobalVariables = context.GetContract(item.GlobalVariables);
			Cutscene = context.GetContract(item.Cutscene);
            RepeatCounters = Repeat.ToDictionary();
		}

		#endregion
	}
}

