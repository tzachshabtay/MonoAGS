using System;
using System.IO;
using AGS.API;

namespace AGS.Engine
{
	public class AGSResource : IResource
	{
		public AGSResource(string id, Stream stream)
		{
			ID = id;
			Stream = stream;
		}

		public string ID { get; private set; }
		public Stream Stream { get; private set; }
	}
}

