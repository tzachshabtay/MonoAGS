using System;
using System.IO;

namespace AGS.Engine
{
	public interface ISoundDecoder
	{
		ISoundData Decode(Stream stream);
	}
}

