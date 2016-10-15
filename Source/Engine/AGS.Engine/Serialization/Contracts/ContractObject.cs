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

        [ProtoMember(12)]
        public float InitialWidth { get; set; }

        [ProtoMember(13)]
        public float InitialHeight { get; set; }

        [ProtoMember(14)]
        public Tuple<float, float, float> Location { get; set; }

        [ProtoMember(15)]
        public bool IsPixelPerfect { get; set; }

        [ProtoMember(16)]
        public float ScaleX { get; set; }

        [ProtoMember(17)]
        public float ScaleY { get; set; }

        [ProtoMember(18)]
        public float Angle { get; set; }

        [ProtoMember(19)]
        public uint Tint { get; set; }

        [ProtoMember(20)]
        public Tuple<float, float> Anchor { get; set; }

        [ProtoMember(21)]
        public Contract<IImage> Image { get; set; }

        //todo: support custom renderer deserialization
        [ProtoMember(22)]
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
                obj.ScaleBy(ScaleX, ScaleY);
            }
            obj.Location = new AGSLocation(Location.Item1, Location.Item2, Location.Item3);
            obj.Anchor = new PointF(Anchor.Item1, Anchor.Item2);
            obj.Angle = Angle;
            obj.Tint = Color.FromHexa(Tint);

            obj.PixelPerfect(IsPixelPerfect);            
            AnimationContainer.ToItem(context, obj);
            if (obj.Animation.Frames.Count > 0)
                obj.ScaleBy(obj.ScaleX, obj.ScaleY);
            obj.RenderLayer = RenderLayer.ToItem(context);
			if (WalkPoint != null)
			{
				obj.WalkPoint = new PointF (WalkPoint.Item1, WalkPoint.Item2);
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
				WalkPoint = new Tuple<float, float> (item.WalkPoint.Value.X, item.WalkPoint.Value.Y);
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
            if (item.Width != 0f)
            {
                var scaleX = item.ScaleX;
                var scaleY = item.ScaleY;
                item.ResetScale();
                InitialWidth = item.Width;
                InitialHeight = item.Height;
                item.ScaleBy(scaleX, scaleY);
            }
            Image = new Contract<IImage>();
            Image.FromItem(context, item.Image);
            Anchor = new Tuple<float, float>(item.Anchor.X, item.Anchor.Y);
            Tint = item.Tint.Value;
            Angle = item.Angle;
            ScaleX = item.ScaleX;
            ScaleY = item.ScaleY;
            IsPixelPerfect = item.PixelPerfectHitTestArea != null;
            Location = new Tuple<float, float, float>(item.X, item.Y, item.Z);
            CustomRenderer = item.CustomRenderer == null ? null : item.CustomRenderer.GetType().Name;
        }

		#endregion
	}
}

