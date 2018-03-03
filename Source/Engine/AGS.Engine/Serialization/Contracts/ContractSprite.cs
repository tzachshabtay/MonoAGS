using System;
using ProtoBuf;

using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractSprite : IContract<ISprite>
	{
		public ContractSprite()
		{
		}

		[ProtoMember(1)]
		public Tuple<float, float, float> Location { get; set; }

		[ProtoMember(2)]
		public float ScaleX { get; set; }

		[ProtoMember(3)]
		public float ScaleY { get; set; }

		[ProtoMember(4)]
		public float Angle { get; set; }

		[ProtoMember(5)]
		public uint Tint { get; set; }

		[ProtoMember(6)]
        public Tuple<float, float> Pivot { get; set; }

		[ProtoMember(7)]
		public Contract<IImage> Image { get; set; }

		//todo: support custom renderer deserialization
		[ProtoMember(8)]
		public string CustomRenderer { get; set; }

		public ISprite ToItem(AGSSerializationContext context)
		{
			ISprite sprite = context.Factory.Graphics.GetSprite();
			ToItem(context, sprite);
			return sprite;
		}

		public void ToItem(AGSSerializationContext context, ISprite sprite)
		{
			var image = Image.ToItem(context);
			if (image != null)
			{
				sprite.Image = image;
                sprite.Scale = new PointF(ScaleX, ScaleY);
			}
			sprite.Location = new AGSLocation (Location.Item1, Location.Item2, Location.Item3);
			sprite.Pivot = new PointF (Pivot.Item1, Pivot.Item2);
			sprite.Angle = Angle;
			sprite.Tint = Color.FromHexa(Tint);
		}

		public void FromItem(AGSSerializationContext context, ISprite sprite)
		{
			Image = new Contract<IImage> ();
			Image.FromItem(context, sprite.Image);
			Pivot = new Tuple<float, float> (sprite.Pivot.X, sprite.Pivot.Y);
			Tint = sprite.Tint.Value;
			Angle = sprite.Angle;
			ScaleX = sprite.ScaleX;
			ScaleY = sprite.ScaleY;
			Location = new Tuple<float, float, float> (sprite.X, sprite.Y, sprite.Z);
			CustomRenderer = sprite.CustomRenderer == null ? null : sprite.CustomRenderer.GetType().Name;
		}
	}
}

