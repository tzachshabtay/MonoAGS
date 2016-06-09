using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IAudioClip : ISoundProperties, ISoundPlayer
	{
		string ID { get; }
	}
}

