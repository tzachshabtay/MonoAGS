namespace AGS.API
{
    /// <summary>
    /// These are areas that indicate that the background graphic of the room should be in front of the 
    /// characters/objects. When rendering the engine will actually crop those up and render them on top.
    /// </summary>
    [RequiredComponent(typeof(IArea))]
    public interface IWalkBehindArea : IComponent
	{
        /// <summary>
        /// The baseline is a horizontal line, which tells the game where the character has to be in order to be 
        /// drawn behind the area. 
        /// For example, if you had a table in the middle of the room, you'd only want him drawn behind the table 
        /// if he was standing behind it.
        /// You normally place a baseline at the lowest point of the walk-behind area.
        /// </summary>
        /// <value>The baseline.</value>
		float? Baseline { get; set; }
	}
}

