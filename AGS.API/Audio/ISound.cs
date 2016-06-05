using System.Threading.Tasks;

namespace AGS.API
{
	public interface ISound : ISoundProperties
	{
		bool IsPaused { get; }
		bool IsLooping { get; }
		bool HasCompleted { get; }
		Task Completed { get; }

		/// <summary>
		/// Gets or sets the seek (position within the sound) in seconds.
		/// </summary>
		/// <value>The seek.</value>
		float Seek { get; set; }

		void Pause();
		void Resume();
		void Rewind();
		void Stop();
	}
}

