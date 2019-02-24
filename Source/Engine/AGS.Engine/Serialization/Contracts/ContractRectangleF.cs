using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
    [ProtoContract]
    public class ContractRectangleF : IContract<RectangleF>
    {
        [ProtoMember(1)]
		public float X { get; set; }

		[ProtoMember(2)]
		public float Y { get; set; }

        [ProtoMember(3)]
        public float Width { get; set; }

        [ProtoMember(4)]
        public float Height { get; set; }

		#region IContract implementation

        public RectangleF ToItem(AGSSerializationContext context)
		{
            return new RectangleF(X, Y, Width, Height);
		}

        public void FromItem(AGSSerializationContext context, RectangleF item)
		{
			X = item.X;
			Y = item.Y;
            Width = item.Width;
            Height = item.Height;
		}

		#endregion
	}
}
