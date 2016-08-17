using System.Threading.Tasks;

namespace AGS.API
{
    [RequiredComponent(typeof(ITransformComponent), false)] //needed for speech sound panning and volume adjustment
    [RequiredComponent(typeof(IHasRoom), false)] //needed for speech sound volume adjustment
    public interface ISayBehavior : IComponent
	{
		ISayConfig SpeechConfig { get; }
		IBlockingEvent<BeforeSayEventArgs> OnBeforeSay { get; }

		void Say(string text);
		Task SayAsync(string text);
	}
}

