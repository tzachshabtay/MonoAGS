using System;
using ProtoBuf;
using AGS.API;
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
            var loadConfig = LoadConfig.ToItem(context);
			var spriteSheet = SpriteSheet.ToItem(context);
            if (context.Textures.TryGetValue(ID, out var texture))
            {
                return getImage(texture, spriteSheet, loadConfig);
            }

            if (spriteSheet != null)
			{
				context.Factory.Graphics.LoadAnimationFromSpriteSheet(spriteSheet, null, loadConfig);
                if (context.Textures.TryGetValue(ID, out texture))
				{
					return getImage(texture, spriteSheet, loadConfig);
				}
			}
			try
			{
                texture = context.Factory.Graphics.LoadImage(ID, loadConfig).Texture;
				context.Textures.Add(ID, texture);
				return getImage(texture, spriteSheet, loadConfig);
			}
			catch (ArgumentException e)
			{
				Debug.WriteLine($"Failed to load image: {ID}. Exception: {e.ToString()}");
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

        private IImage getImage(ITexture texture, ISpriteSheet spriteSheet, ILoadImageConfig loadConfig)
        {
            return new GLImage(AGSGame.Device.BitmapLoader.Load((int)Width, (int)Height), ID, texture, spriteSheet, loadConfig);
        }
    }
}

