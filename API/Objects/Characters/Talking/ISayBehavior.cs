using System.Threading.Tasks;

namespace AGS.API
{
	public interface ISayBehavior : IComponent
	{
		ISayConfig SpeechConfig { get; }

		void Say(string text);
		Task SayAsync(string text);
	}
}

