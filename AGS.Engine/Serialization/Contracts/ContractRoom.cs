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
		public IList<IContract<IArea>> WalkableAreas { get; set; }

		[ProtoMember(8)]
		public IList<IContract<IWalkBehindArea>> WalkBehindAreas { get; set; }

		[ProtoMember(9)]
		public IList<IContract<IScalingArea>> ScalingAreas { get; set; }

		[ProtoMember(10)]
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
			if (WalkableAreas != null)
			{
				foreach (var area in WalkableAreas)
				{
					room.WalkableAreas.Add(area.ToItem(context));
				}
			}
			if (WalkBehindAreas != null)
			{
				foreach (var area in WalkBehindAreas)
				{
					room.WalkBehindAreas.Add(area.ToItem(context));
				}
			}
			if (ScalingAreas != null)
			{
				foreach (var area in ScalingAreas)
				{
					room.ScalingAreas.Add(area.ToItem(context));
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

			WalkableAreas = new List<IContract<IArea>> (item.WalkableAreas.Count);
			foreach (var area in item.WalkableAreas)
			{
				WalkableAreas.Add(context.GetContract(area));
			}

			WalkBehindAreas = new List<IContract<IWalkBehindArea>> (item.WalkBehindAreas.Count);
			foreach (var area in item.WalkBehindAreas)
			{
				WalkBehindAreas.Add(context.GetContract(area));
			}

			ScalingAreas = new List<IContract<IScalingArea>> (item.ScalingAreas.Count);
			foreach (var area in item.ScalingAreas)
			{
				ScalingAreas.Add(context.GetContract(area));
			}

			Edges = context.GetContract((IAGSEdges)item.Edges);
		}

		#endregion
	}
}

