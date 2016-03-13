using System;
using ProtoBuf;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractSlider : IContract<IObject>
	{
		static ContractSlider()
		{
			ContractsFactory.RegisterFactory(typeof(ISlider), () => new ContractSlider ());
		}

		public ContractSlider()
		{
		}
			
		[ProtoMember(1)]
		public ContractObject Object { get; set; }

		[ProtoMember(2)]
		public ContractObject Graphics { get; set; }

		[ProtoMember(3)]
		public ContractObject HandleGraphics { get; set; }

		[ProtoMember(4)]
		public ContractLabel Label { get; set; }

		[ProtoMember(5)]
		public float MinValue { get; set; }

		[ProtoMember(6)]
		public float MaxValue { get; set; }

		[ProtoMember(7)]
		public float Value { get; set; }

		[ProtoMember(8)]
		public bool IsHorizontal { get; set; }

		#region IContract implementation

		public IObject ToItem(AGSSerializationContext context)
		{
			IObject obj = Object.ToItem(context);
			TypedParameter objParam = new TypedParameter (typeof(IObject), obj);
			IPanel panel = context.Resolver.Container.Resolve<IPanel>(objParam);
			TypedParameter panelParam = new TypedParameter (typeof(IPanel), panel);
			ISlider slider = context.Resolver.Container.Resolve<ISlider>(panelParam);

			Graphics.Parent = null;
			slider.Graphics = Graphics.ToItem(context);

			HandleGraphics.Parent = null;
			slider.HandleGraphics = HandleGraphics.ToItem(context);

			slider.Label = (ILabel)Label.ToItem(context);
			slider.MinValue = MinValue;
			slider.MaxValue = MaxValue;
			slider.Value = Value;
			slider.IsHorizontal = IsHorizontal;
			slider.TreeNode.StealParent(obj.TreeNode);
			slider.Graphics.TreeNode.SetParent(slider.TreeNode);
			slider.HandleGraphics.TreeNode.SetParent(slider.TreeNode);

			return slider;
		}

		public void FromItem(AGSSerializationContext context, IObject obj)
		{
			ISlider item = (ISlider)obj;
			Object = new ContractObject ();
			Object.FromItem(context, item);

			Graphics = new ContractObject ();
			Graphics.Parent = this;
			Graphics.FromItem(context, item.Graphics);

			HandleGraphics = new ContractObject ();
			HandleGraphics.Parent = this;
			HandleGraphics.FromItem(context, item.HandleGraphics);

			Label = new ContractLabel ();
			if (item.Label != null) Label.FromItem(context, item.Label);

			MinValue = item.MinValue;
			MaxValue = item.MaxValue;
			Value = item.Value;
			IsHorizontal = item.IsHorizontal;
		}

		#endregion
	}
}

