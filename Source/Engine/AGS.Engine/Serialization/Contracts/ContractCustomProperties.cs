using System;
using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractCustomProperties : IContract<ICustomProperties>
	{
		public ContractCustomProperties()
		{
		}

		[ProtoMember(1)]
		public IDictionary<string, string> Strings { get; set; }

		[ProtoMember(2)]
		public IDictionary<string, int> Ints { get; set; }

		[ProtoMember(3)]
		public IDictionary<string, bool> Bools { get; set; }

		[ProtoMember(4)]
		public IDictionary<string, float> Floats { get; set; }

		#region IContract implementation

		public ICustomProperties ToItem(AGSSerializationContext context)
		{
			AGSCustomProperties props = new AGSCustomProperties ();
			if (Strings != null) foreach (var x in Strings) props.SetString(x.Key, x.Value);
			if (Ints != null) foreach (var x in Ints) props.SetInt(x.Key, x.Value);
			if (Bools != null) foreach (var x in Bools) props.SetBool(x.Key, x.Value);
			if (Floats != null) foreach (var x in Floats) props.SetFloat(x.Key, x.Value);

			return props;
		}

		public void FromItem(AGSSerializationContext context, ICustomProperties item)
		{
			Strings = item.AllStrings();
			Ints = item.AllInts();
			Bools = item.AllBooleans();
			Floats = item.AllFloats();
		}

		#endregion
	}
}

