using System;

namespace API
{
	public interface ICustomSerializable
	{
		void Load(string contents);
		ICustomSerializableData Save();
	}
}

