using System.Threading.Tasks;

namespace AGS.API
{
    public interface ISound
	{
		string ID { get; }
		int Volume { get; set; }

		void Play();
		Task PlayAsync();

		void Pause();
		void Resume();
	}
}

