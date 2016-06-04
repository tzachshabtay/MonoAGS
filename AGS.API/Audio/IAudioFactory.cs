using System.Threading.Tasks;

namespace AGS.API
{
    public interface IAudioFactory
	{
		IAudioClip LoadAudioClip(string filePath, string id = null);
		Task<IAudioClip> LoadAudioClipAsync(string filePath, string id = null);
	}
}

