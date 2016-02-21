using System;
using ProtoBuf;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractSlider : IContract<ISlider>
	{
		public ContractSlider()
		{
		}

		[ProtoMember(1)]
		public Contract<IObject> Object { get; set; }

		[ProtoMember(2)]
		public Contract<IObject> Graphics { get; set; }

		[ProtoMember(3)]
		public Contract<IObject> HandleGraphics { get; set; }

		[ProtoMember(4)]
		public Contract<ILabel> Label { get; set; }

		[ProtoMember(5)]
		public float MinValue { get; set; }

		[ProtoMember(6)]
		public float MaxValue { get; set; }

		[ProtoMember(7)]
		public float Value { get; set; }

		[ProtoMember(8)]
		public bool IsHorizontal { get; set; }

		#region IContract implementation

		public ISlider ToItem(AGSSerializationContext context)
		{
			IObject obj = Object.ToItem(context);
			TypedParameter objParam = new TypedParameter (typeof(IObject), obj);
			ISlider slider = context.Resolver.Container.Resolve<ISlider>(objParam);
			slider.Graphics = Graphics.ToItem(context);
			slider.HandleGraphics = HandleGraphics.ToItem(context);
			slider.Label = Label.ToItem(context);
			slider.MinValue = MinValue;
			slider.MaxValue = MaxValue;
			slider.Value = Value;
			slider.IsHorizontal = IsHorizontal;
			slider.TreeNode.StealParent(obj.TreeNode);

			return slider;
		}

		public void FromItem(AGSSerializationContext context, ISlider item)
		{
			Object = new Contract<IObject> ();
			Object.FromItem(context, item);

			Graphics = new Contract<IObject> ();
			Graphics.FromItem(context, item.Graphics);

			HandleGraphics = new Contract<IObject> ();
			HandleGraphics.FromItem(context, item.HandleGraphics);

			Label = new Contract<ILabel> ();
			Label.FromItem(context, item.Label);

			MinValue = item.MinValue;
			MaxValue = item.MaxValue;
			Value = item.Value;
			IsHorizontal = item.IsHorizontal;
		}

		#endregion
	}
}

