using System;
using ProtoBuf;
using System.Drawing;
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
		public bool IsPixelPerfect { get; set; }

		[ProtoMember(3)]
		public float ScaleX { get; set; }

		[ProtoMember(4)]
		public float ScaleY { get; set; }

		[ProtoMember(5)]
		public float Angle { get; set; }

		[ProtoMember(6)]
		public Color Tint { get; set; }

		[ProtoMember(7)]
		public Tuple<float, float> Anchor { get; set; }

		[ProtoMember(8)]
		public Contract<IImage> Image { get; set; }

		//todo: support custom renderer deserialization
		[ProtoMember(9)]
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
				sprite.ScaleBy(ScaleX, ScaleY);
			}
			sprite.Location = new AGSLocation (Location.Item1, Location.Item2, Location.Item3);
			sprite.Anchor = new AGSPoint (Anchor.Item1, Anchor.Item2);
			sprite.Angle = Angle;
			sprite.Tint = (AGSColor)Tint;

			sprite.PixelPerfect(IsPixelPerfect);
		}

		public void FromItem(AGSSerializationContext context, ISprite sprite)
		{
			Image = new Contract<IImage> ();
			Image.FromItem(context, sprite.Image);
			Anchor = new Tuple<float, float> (sprite.Anchor.X, sprite.Anchor.Y);
			Tint = sprite.Tint == null ? Color.White : Color.FromArgb(sprite.Tint.A, sprite.Tint.R, sprite.Tint.G, sprite.Tint.B);
			Angle = sprite.Angle;
			ScaleX = sprite.ScaleX;
			ScaleY = sprite.ScaleY;
			IsPixelPerfect = sprite.PixelPerfectHitTestArea != null;
			Location = new Tuple<float, float, float> (sprite.X, sprite.Y, sprite.Z);
			CustomRenderer = sprite.CustomRenderer == null ? null : sprite.CustomRenderer.GetType().Name;
		}
	}
}

