using System.Threading.Tasks;

namespace AGS.API
{
	public interface ISayBehavior : IComponent
	{
		ISayConfig SpeechConfig { get; }
		IBlockingEvent<BeforeSayEventArgs> OnBeforeSay { get; }

		void Say(string text);
		Task SayAsync(string text);
	}
}

