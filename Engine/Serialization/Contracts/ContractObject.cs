using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract(AsReferenceDefault = true)]
	public class ContractObject : IContract<IObject>
	{
		private IObject _obj;

		static ContractObject()
		{
			ContractsFactory.RegisterFactory(typeof(IObject), () => new ContractObject ());
		}

		public ContractObject()
		{
		}

		[ProtoMember(1)]
		public IContract<IRenderLayer> RenderLayer { get; set; }

		[ProtoMember(2)]
		public Tuple<float, float> WalkPoint { get; set; }

		[ProtoMember(3)]
		public IContract<ICustomProperties> Properties { get; set; }

		[ProtoMember(4)]
		public bool Enabled { get; set; }

		[ProtoMember(5)]
		public string Hotspot { get; set; }

		[ProtoMember(6)]
		public bool IgnoreViewport { get; set; }

		[ProtoMember(7)]
		public bool IgnoreScalingArea { get; set; }

		[ProtoMember(8)]
		public ContractAnimationContainer AnimationContainer { get; set; }

		[ProtoMember(9, AsReference = true)]
		public IContract<IObject> Parent { get; set; }

		[ProtoMember(10)]
		public string ID { get; set; }

		[ProtoMember(11)]
		public bool Visible { get; set; }

		//todo: save object's previous room

		#region IContract implementation

		public IObject ToItem(AGSSerializationContext context)
		{
			if (_obj != null) return _obj;

			_obj = context.Factory.Object.GetObject(ID, AnimationContainer.ToItem(context));
			ToItem(context, _obj);

			return _obj;
		}

		public void ToItem(AGSSerializationContext context, IObject obj)
		{
			obj.RenderLayer = RenderLayer.ToItem(context);
			if (WalkPoint != null)
			{
				obj.WalkPoint = new AGSPoint (WalkPoint.Item1, WalkPoint.Item2);
			}
			obj.Properties.CopyFrom(Properties.ToItem(context));
			obj.Enabled = Enabled;
			obj.Hotspot = Hotspot;
			obj.IgnoreViewport = IgnoreViewport;
			obj.IgnoreScalingArea = IgnoreScalingArea;
			obj.Visible = Visible;

			if (Parent != null)
			{
				var parent = Parent.ToItem(context);
				obj.TreeNode.SetParent(parent.TreeNode);
			}
		}
			
		public void FromItem(AGSSerializationContext context, IObject item)
		{
			ID = item.ID;
			RenderLayer = context.GetContract(item.RenderLayer);

			Properties = context.GetContract(item.Properties);

			AnimationContainer = new ContractAnimationContainer ();
			AnimationContainer.FromItem(context, item);

			if (item.WalkPoint != null)
			{
				WalkPoint = new Tuple<float, float> (item.WalkPoint.X, item.WalkPoint.Y);
			}
			Enabled = item.UnderlyingEnabled;
			Visible = item.UnderlyingVisible;
			Hotspot = item.Hotspot;
			IgnoreViewport = item.IgnoreViewport;
			IgnoreScalingArea = item.IgnoreScalingArea;
			if (Parent == null && item.TreeNode != null && item.TreeNode.Parent != null)
			{
				Parent = context.GetContract(item.TreeNode.Parent);
			}
		}

		#endregion
	}
}

