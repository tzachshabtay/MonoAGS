using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;
using System.Diagnostics;

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

		[ProtoMember(4)]
		public IContract<ILoadImageConfig> LoadConfig { get; set; }

		[ProtoMember(5, AsReference = true)]
		public IContract<ISpriteSheet> SpriteSheet { get; set; }

		public IImage ToItem(AGSSerializationContext context)
		{
			if (string.IsNullOrEmpty(ID)) return new EmptyImage (Width, Height);
			GLImage image;
			if (context.Textures.TryGetValue(ID, out image))
			{
				return image;
			}
			var loadConfig = LoadConfig.ToItem(context);
			var spriteSheet = SpriteSheet.ToItem(context);
			if (spriteSheet != null)
			{
				context.Factory.Graphics.LoadAnimationFromSpriteSheet(spriteSheet, 4, null, loadConfig);
				if (context.Textures.TryGetValue(ID, out image))
				{
					return image;
				}
			}
			try
			{
				image = ((GLGraphicsFactory)context.Factory.Graphics).LoadImageInner(ID, loadConfig);
				context.Textures.Add(ID, image);
				return image;
			}
			catch (ArgumentException e)
			{
				Debug.WriteLine(string.Format("Failed to load image: {0}. Exception: {1}", ID, e.ToString()));
				return new EmptyImage (Width, Height);
			}
		}

		public void FromItem(AGSSerializationContext context, IImage image)
		{
			ID = image.ID;
			Width = image.Width;
			Height = image.Height;
			LoadConfig = context.GetContract(image.LoadConfig);
		}
	}
}

