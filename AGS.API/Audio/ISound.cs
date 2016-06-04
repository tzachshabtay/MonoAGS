using System.Threading.Tasks;

namespace AGS.API
{
	public interface ISound : ISoundProperties
	{
		bool IsPaused { get; }
		bool IsLooping { get; }
		bool HasCompleted { get; }
		Task Completed { get; }

		void Pause();
		void Resume();
		void Rewind();
		void Stop();
	}
}

