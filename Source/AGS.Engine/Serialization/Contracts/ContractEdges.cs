using System;
using ProtoBuf;
using AGS.API;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractEdges : IContract<IAGSEdges>
	{
		public ContractEdges()
		{
		}

		[ProtoMember(1)]
		public Contract<IEdge> Left { get; set; }

		[ProtoMember(2)]
		public Contract<IEdge> Right { get; set; }

		[ProtoMember(3)]
		public Contract<IEdge> Top { get; set; }

		[ProtoMember(4)]
		public Contract<IEdge> Bottom { get; set; }

		#region IContract implementation

		public IAGSEdges ToItem(AGSSerializationContext context)
		{
			AGSEdges edges = new AGSEdges (Left.ToItem(context), Right.ToItem(context),
				                 Top.ToItem(context), Bottom.ToItem(context));

			return edges;
		}

		public void FromItem(AGSSerializationContext context, IAGSEdges item)
		{
			Left = fromItem(context, item.Left);
			Bottom = fromItem(context, item.Bottom);
			Right = fromItem(context, item.Right);
			Top = fromItem(context, item.Top);
		}

		#endregion

		private Contract<IEdge> fromItem(AGSSerializationContext context, IEdge edge)
		{
			var contract = new Contract<IEdge> ();
			contract.FromItem(context, edge);
			return contract;
		}
	}
}

