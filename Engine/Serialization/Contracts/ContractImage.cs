using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractImage : IContract<IImage>
	{
		public ContractImage()
		{
		}

		[ProtoMember(1)]
		public float Width { get; set; } 

		[ProtoMember(2)]
		public float Height{ get; set; }

		[ProtoMember(3)]
		public string ID { get; set; }

		public IImage ToItem(AGSSerializationContext context)
		{
			if (string.IsNullOrEmpty(ID)) return new EmptyImage (Width, Height);
			GLImage image;
			if (context.Textures.TryGetValue(ID, out image))
			{
				return image;
			}
			//todo: support spritesheets for textures that are not in-game yet (i.e support loadconfig)
			try
			{
				image = ((GLGraphicsFactory)context.Factory.Graphics).LoadImageInner(ID);
				return image;
			}
			catch (ArgumentException)
			{
				return new EmptyImage (Width, Height);
			}
		}

		public void FromItem(AGSSerializationContext context, IImage image)
		{
			ID = image.ID;
			Width = image.Width;
			Height = image.Height;
		}
	}
}

