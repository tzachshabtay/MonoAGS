using System;
using AGS.API;
using ProtoBuf;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractMask : IContract<IMask>
	{
	    [ProtoMember(1)]
		public bool[] Mask { get; set; }

		[ProtoMember(2)]
		public int MaskWidth { get; set; }

		[ProtoMember(3)]
		public int MaskHeight { get; set; }

		[ProtoMember(4)]
		public Contract<IObject> DebugObject { get; set; }

		#region IContract implementation

		public IMask ToItem(AGSSerializationContext context)
		{
			bool[][] array = new bool[MaskWidth][];
			for (int x = 0; x < MaskWidth; x++)
			{
				array[x] = new bool[MaskHeight];
				for (int y = 0; y < MaskHeight; y++)
				{
					array[x][y] = Mask[y * MaskWidth + x];
				}
			}

			AGSMask mask = new AGSMask (array, DebugObject.ToItem(context));
			return mask;
		}

		public void FromItem(AGSSerializationContext context, IMask item)
		{
			bool[][] mask = item.AsJaggedArray();
			MaskWidth = mask.Length;
			MaskHeight = mask[0].Length;
			Mask = new bool[MaskWidth * MaskHeight];
			for (int x = 0; x < MaskWidth; x++)
			{
				for (int y = 0; y < MaskHeight; y++)
				{
					Mask[y * MaskWidth + x] = mask[x][y];
				}
			}

			DebugObject = new Contract<IObject> ();
			DebugObject.FromItem(context, item.DebugDraw);
		}

		#endregion
	}
}

