using System.IO;

namespace AGS.API
{
    public interface IResource
	{
		string ID { get; }
		Stream Stream { get; }
	}
}

