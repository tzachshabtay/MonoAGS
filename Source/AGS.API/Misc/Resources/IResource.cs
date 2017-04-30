using System.IO;

namespace AGS.API
{
    /// <summary>
    /// A resource is any asset which is loaded from a stream of data (could be from a file, or embedded in memory, or network stream,
    /// or any other stream).
    /// </summary>
    public interface IResource
	{
        /// <summary>
        /// A unique identifer for the resource.
        /// </summary>
        /// <value>The identifier.</value>
		string ID { get; }

        /// <summary>
        /// The stream of data associated with the resource.
        /// </summary>
        /// <value>The stream.</value>
		Stream Stream { get; }
	}
}

