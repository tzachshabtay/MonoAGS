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
		public IContract<IViewport> Viewport { get; set; }

		[ProtoMember(4)]
		public IContract<IObject> Background  { get; set; }

		[ProtoMember(5, AsReference = true)]
		public IList<IContract<IObject>> Objects { get; set; }

		[ProtoMember(6)]
		public IContract<ICustomProperties> Properties { get; set; }

		[ProtoMember(7)]
		public IList<IContract<IArea>> Areas { get; set; }

		[ProtoMember(8)]
		public IContract<IAGSEdges> Edges { get; set; }

		#region IContract implementation

		public IRoom ToItem(AGSSerializationContext context)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), ID);
			TypedParameter viewParam = new TypedParameter (typeof(IViewport), Viewport.ToItem(context));
			TypedParameter edgesParam = new TypedParameter (typeof(IAGSEdges), Edges.ToItem(context));
			IRoom room = context.Resolver.Container.Resolve<IRoom>(idParam, viewParam, edgesParam);

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
			return room;
		}

		public void FromItem(AGSSerializationContext context, IRoom item)
		{
			ID = item.ID;
			ShowPlayer = item.ShowPlayer;

			Viewport = context.GetContract(item.Viewport);

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
		}

		#endregion
	}
}

