using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface ISoundFactory
	{
		ISound LoadSound(string filePath, string id = null);
		Task<ISound> LoadSoundAsync(string filePath, string id = null);
	}
}

