using System.Threading.Tasks;

namespace AGS.API
{
	public interface ISound : ISoundProperties
	{
		int SourceID { get; }
        /// <summary>
        /// Is the sound a valid sound and expected to play properly, or is there some audio problem and this is a dummy object?
        /// </summary>
        bool IsValid { get; }
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

