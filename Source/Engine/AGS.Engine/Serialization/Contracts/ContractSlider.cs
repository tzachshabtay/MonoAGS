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
        public SliderDirection Direction { get; set; }

        [ProtoMember(9)]
        public bool AllowKeyboardControl { get; set; }

		#region IContract implementation

		public IObject ToItem(AGSSerializationContext context)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), Object.ID);
			ISlider slider = context.Resolver.Container.Resolve<ISlider>(idParam);
			Object.ToItem(context, slider);

			Graphics.Parent = null;

			HandleGraphics.Parent = null;

			slider.Label = (ILabel)Label.ToItem(context);
			slider.MinValue = MinValue;
			slider.MaxValue = MaxValue;
			slider.Value = Value;
            slider.Direction = Direction;
            slider.AllowKeyboardControl = AllowKeyboardControl;

			context.Rewire(state =>
			{
				IObject obj = state.Find<IObject>(Graphics.ID);
				if (obj != null) state.UI.Remove(obj);
				obj = state.Find<IObject>(HandleGraphics.ID);
				if (obj != null) state.UI.Remove(obj);

				slider.Graphics = Graphics.ToItem(context);
				slider.HandleGraphics = HandleGraphics.ToItem(context);
				slider.Graphics.TreeNode.SetParent(slider.TreeNode);
				slider.HandleGraphics.TreeNode.SetParent(slider.TreeNode);
			});

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
            Direction = item.Direction;
            AllowKeyboardControl = item.AllowKeyboardControl;
		}

		#endregion
	}
}

