using System;

namespace AGS.API
{
	public interface ICustomSerializableData
	{
		/// <summary>
		/// Type must implement ICustomSerializable
		/// </summary>
		/// <value>The type.</value>
		Type Type { get; }

		string Content { get; }
	}
}

