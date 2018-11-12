using ProtoBuf;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	[ProtoContract]
	public class ContractCustomProperties : IContract<ICustomProperties>
	{
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
            if (Strings != null) foreach (var x in Strings) props.Strings.SetValue(x.Key, x.Value);
            if (Ints != null) foreach (var x in Ints) props.Ints.SetValue(x.Key, x.Value);
            if (Bools != null) foreach (var x in Bools) props.Bools.SetValue(x.Key, x.Value);
            if (Floats != null) foreach (var x in Floats) props.Floats.SetValue(x.Key, x.Value);

			return props;
		}

		public void FromItem(AGSSerializationContext context, ICustomProperties item)
		{
            Strings = item.Strings.AllProperties();
            Ints = item.Ints.AllProperties();
            Bools = item.Bools.AllProperties();
            Floats = item.Floats.AllProperties();
		}

		#endregion
	}
}

