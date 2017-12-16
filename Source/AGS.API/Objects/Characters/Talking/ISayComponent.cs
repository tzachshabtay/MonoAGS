using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// A component which adds the ability to speak for an entity.
    /// </summary>
    /// <seealso cref="IComponent"/>
    [RequiredComponent(typeof(ITranslateComponent), false)] //needed for speech sound panning and volume adjustment
    [RequiredComponent(typeof(IHasRoomComponent), false)] //needed for speech sound volume adjustment
    [RequiredComponent(typeof(IOutfitComponent))]
    [RequiredComponent(typeof(IFaceDirectionComponent))]
    public interface ISayComponent : IComponent
	{
        /// <summary>
        /// Gets the speech configuration.
        /// </summary>
        /// <value>The speech config.</value>
		ISayConfig SpeechConfig { get; }

        /// <summary>
        /// An event which allows you to manipulate the text label just before it is being rendered,
        /// and to allow you to have a custom skip text implementation.
        /// </summary>
        /// <value>The on before say.</value>
		IBlockingEvent<BeforeSayEventArgs> OnBeforeSay { get; }

        /// <summary>
        /// Say the specified text.
        /// This basically does the following:
        /// 1. Puts text on the screen for some time
        /// 2. Change the character animation to the speaking animation
        /// 3. Optionally display a portrait of the character
        /// 4. Optionally play an audio clip
        /// You can configure how the text is rendered (color, font, etc: see text rendering section for more on the all the options that you have for text rendering), when the text is skipped
        /// (by time and how much, by mouse, both, or none which means you implement text skipping yourself), a label to host the text with a border(see the section about borders) and background color,
        /// and a text offset to offset the location of the text.That offset will be relative to the default text display location which will be selected by the engine: The engine, by default, tries to place
        /// the text above the character and a little to the side, unless the text won't fit the screen, then it changes the location to ensure the text fits the screen.
        /// If you don't like that behavior you can implement your own text location by implementing the <see cref="ISayLocationProvider"/>  interface (and hooking it up, see the section about customizability).
        /// 
        /// As for the portrait, you can configure the object which shows the portrait(this is a standard object, so it can has everything a normal object has, including animations, scaling, rotations, etc),
        /// plus a strategy on where to position the portrait(SpeakerPosition- on the top, either left or right based on where the speaker is standing, Alternating- top right, then top left, then top right, etc
        /// or Custom which leaves the portrait positioning to you) and custom offsets for both the portrait and for the text from the portrait.
        /// 
        /// In order to play an audio clip, the text you pass to the speech method should have a number at the beginning prefixed by an ampersand, for example, "&amp;17 Hello there!".
        /// If the number exists, the engine will look for an audio file with the first four capital letters of your character's id followed by the number. So if the line was said by "Christopher",
        /// the engine will look for the file "CHRI17". Those files should be placed in a special folder, under Assets -> Speech -> English.
        /// Note: In future revisions we're planning on introducing multi-language support, and also other ways of building the speech "database".
        /// If the engine does not find the file it will not play an audio clip, and in any way the "&amp;17" will be stripped away from the text and will not be displayed on screen. 
        /// </summary>
        /// <example>
        /// <code>
        /// cHero.Say("&amp;17 Hello there!"); //Will say "Hello there!" and will play "JASO17.ogg" (assuming the character name is "Jason" and JASO17.ogg exists in Assets/Speech/English)
        /// </code>
        /// </example>
        /// <param name="text">Text.</param>
		void Say(string text);

        /// <summary>
        /// Say the specified text asynchronously.
        /// This basically does the following:
        /// 1. Puts text on the screen for some time
        /// 2. Change the character animation to the speaking animation
        /// 3. Optionally display a portrait of the character
        /// 4. Optionally play an audio clip
        /// You can configure how the text is rendered (color, font, etc: see text rendering section for more on the all the options that you have for text rendering), when the text is skipped
        /// (by time and how much, by mouse, both, or none which means you implement text skipping yourself), a label to host the text with a border(see the section about borders) and background color,
        /// and a text offset to offset the location of the text.That offset will be relative to the default text display location which will be selected by the engine: The engine, by default, tries to place
        /// the text above the character and a little to the side, unless the text won't fit the screen, then it changes the location to ensure the text fits the screen.
        /// If you don't like that behavior you can implement your own text location by implementing the <see cref="ISayLocationProvider"/>  interface (and hooking it up, see the section about customizability).
        /// 
        /// As for the portrait, you can configure the object which shows the portrait(this is a standard object, so it can has everything a normal object has, including animations, scaling, rotations, etc),
        /// plus a strategy on where to position the portrait(SpeakerPosition- on the top, either left or right based on where the speaker is standing, Alternating- top right, then top left, then top right, etc
        /// or Custom which leaves the portrait positioning to you) and custom offsets for both the portrait and for the text from the portrait.
        /// 
        /// In order to play an audio clip, the text you pass to the speech method should have a number at the beginning prefixed by an ampersand, for example, "&amp;17 Hello there!".
        /// If the number exists, the engine will look for an audio file with the first four capital letters of your character's id followed by the number. So if the line was said by "Christopher",
        /// the engine will look for the file "CHRI17". Those files should be placed in a special folder, under Assets -> Speech -> English.
        /// Note: In future revisions we're planning on introducing multi-language support, and also other ways of building the speech "database".
        /// If the engine does not find the file it will not play an audio clip, and in any way the "&amp;17" will be stripped away from the text and will not be displayed on screen. 
        /// </summary>
        /// <example>
        /// <code>
        /// await cHero.SayAsync("&amp;17 Hello there!"); //Will say "Hello there!" and will play "JASO17.ogg" (assuming the character name is "Jason" and JASO17.ogg exists in Assets/Speech/English)
        /// 
        /// Task waitForTalk = cHero.SayAsync("Walking Home!");
        /// await cHero.WalkAsync(home); //character will walk and talk at the same time.
        /// await waitForTalk; //Waiting for the talk to complete before moving to the next setnence.
        /// cHero.Say("And, I'm home!");
        /// </code>
        /// </example>
        /// <param name="text">Text.</param>
		Task SayAsync(string text);
	}
}

