using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;
using Autofac;

namespace AGS.Engine
{
	[ProtoContract(AsReferenceDefault = true)]
	public class ContractRoom : IContract<IRoom>
	{
		public ContractRoom()
		{
		}

		[ProtoMember(1)]
		public string ID { get; set; }

		[ProtoMember(2)]
		public bool ShowPlayer { get; set; }

		[ProtoMember(3)]
		public IContract<IObject> Background  { get; set; }

		[ProtoMember(4, AsReference = true)]
		public IList<IContract<IObject>> Objects { get; set; }

		[ProtoMember(5)]
		public IContract<ICustomProperties> Properties { get; set; }

		[ProtoMember(6)]
		public IList<IContract<IArea>> Areas { get; set; }

		[ProtoMember(7)]
		public IContract<IAGSEdges> Edges { get; set; }

        [ProtoMember(8)]
        public uint? BackgroundColor { get; set; }

		#region IContract implementation

		public IRoom ToItem(AGSSerializationContext context)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), ID);
			TypedParameter edgesParam = new TypedParameter (typeof(IAGSEdges), Edges.ToItem(context));
			IRoom room = context.Resolver.Container.Resolve<IRoom>(idParam, edgesParam);

			room.ShowPlayer = ShowPlayer;
			room.Background = Background.ToItem(context);
			foreach (var obj in Objects)
			{
				room.Objects.Add(obj.ToItem(context));
			}
			room.Properties.CopyFrom(Properties.ToItem(context));
			if (Areas != null)
			{
				foreach (var area in Areas)
				{
					room.Areas.Add(area.ToItem(context));
				}
			}
			IAGSEdges edges = room.Edges as IAGSEdges;
			if (edges != null)
			{
				edges.FromEdges(Edges.ToItem(context));
			}
            room.BackgroundColor = BackgroundColor == null ? (Color?)null : Color.FromHexa(BackgroundColor.Value);
			return room;
		}

		public void FromItem(AGSSerializationContext context, IRoom item)
		{
			ID = item.ID;
			ShowPlayer = item.ShowPlayer;

			Background = context.GetContract(item.Background);

			Objects = new List<IContract<IObject>> (item.Objects.Count);

			foreach (var obj in item.Objects)
			{
				Objects.Add(context.GetContract(obj));
			}

			Properties = context.GetContract(item.Properties);

			Areas = new List<IContract<IArea>> (item.Areas.Count);
			foreach (var area in item.Areas)
			{
				Areas.Add(context.GetContract(area));
			}

			Edges = context.GetContract((IAGSEdges)item.Edges);
            BackgroundColor = item.BackgroundColor?.Value;
		}

		#endregion
	}
}

