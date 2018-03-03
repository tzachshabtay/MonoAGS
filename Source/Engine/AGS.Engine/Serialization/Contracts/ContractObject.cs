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
		public IContract<ICustomProperties> Properties { get; set; }

		[ProtoMember(3)]
		public bool Enabled { get; set; }

		[ProtoMember(4)]
		public string DisplayName { get; set; }

		[ProtoMember(5)]
		public bool IgnoreViewport { get; set; }

		[ProtoMember(6)]
		public bool IgnoreScalingArea { get; set; }

		[ProtoMember(7)]
		public ContractAnimationComponent AnimationComponent { get; set; }

		[ProtoMember(8, AsReference = true)]
		public IContract<IObject> Parent { get; set; }

		[ProtoMember(9)]
		public string ID { get; set; }

		[ProtoMember(10)]
		public bool Visible { get; set; }

        [ProtoMember(11)]
        public float InitialWidth { get; set; }

        [ProtoMember(12)]
        public float InitialHeight { get; set; }

        [ProtoMember(13)]
        public Tuple<float, float, float> Location { get; set; }

        [ProtoMember(14)]
        public bool IsPixelPerfect { get; set; }

        [ProtoMember(15)]
        public float ScaleX { get; set; }

        [ProtoMember(16)]
        public float ScaleY { get; set; }

        [ProtoMember(17)]
        public float Angle { get; set; }

        [ProtoMember(18)]
        public uint Tint { get; set; }

        [ProtoMember(19)]
        public Tuple<float, float> Pivot { get; set; }

        [ProtoMember(20)]
        public Contract<IImage> Image { get; set; }

        //todo: support custom renderer deserialization
        [ProtoMember(21)]
        public string CustomRenderer { get; set; }

        //todo: save object's previous room

        #region IContract implementation

        public IObject ToItem(AGSSerializationContext context)
		{
			if (_obj != null) return _obj;

			_obj = context.Factory.Object.GetObject(ID);
			ToItem(context, _obj);

			return _obj;
		}

		public void ToItem(AGSSerializationContext context, IObject obj)
		{
            obj.ResetScale(InitialWidth, InitialHeight);
            var image = Image.ToItem(context);
            if (image != null)
            {
                obj.Image = image;
                obj.Scale = new PointF(ScaleX, ScaleY);
            }
            obj.Location = new AGSLocation(Location.Item1, Location.Item2, Location.Item3);
            obj.Pivot = new PointF(Pivot.Item1, Pivot.Item2);
            obj.Angle = Angle;
            obj.Tint = Color.FromHexa(Tint);

            obj.IsPixelPerfect = IsPixelPerfect;
            AnimationComponent.ToItem(context, obj);
            obj.RenderLayer = RenderLayer.ToItem(context);
            obj.Properties.CopyFrom(Properties.ToItem(context));
			obj.Enabled = Enabled;
			obj.DisplayName = DisplayName;
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

			AnimationComponent = new ContractAnimationComponent ();
			AnimationComponent.FromItem(context, item);

			Enabled = item.UnderlyingEnabled;
			Visible = item.UnderlyingVisible;
            DisplayName = item.DisplayName;
			IgnoreViewport = item.IgnoreViewport;
			IgnoreScalingArea = item.IgnoreScalingArea;
			if (Parent == null && item.TreeNode != null && item.TreeNode.Parent != null)
			{
				Parent = context.GetContract(item.TreeNode.Parent);
			}
            if (item.Width != 0f)
            {
                var scaleX = item.ScaleX;
                var scaleY = item.ScaleY;
                item.ResetScale();
                InitialWidth = item.Width;
                InitialHeight = item.Height;
                item.Scale = new PointF(scaleX, scaleY);
            }
            Image = new Contract<IImage>();
            Image.FromItem(context, item.Image);
            Pivot = new Tuple<float, float>(item.Pivot.X, item.Pivot.Y);
            Tint = item.Tint.Value;
            Angle = item.Angle;
            ScaleX = item.ScaleX;
            ScaleY = item.ScaleY;
            IsPixelPerfect = item.IsPixelPerfect;
            Location = new Tuple<float, float, float>(item.X, item.Y, item.Z);
            CustomRenderer = item.CustomRenderer == null ? null : item.CustomRenderer.GetType().Name;
        }

		#endregion
	}
}

