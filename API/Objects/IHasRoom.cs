namespace AGS.API
{
	public interface IHasRoom : IComponent
	{
		IRoom Room { get; }
		IRoom PreviousRoom { get; }

		void ChangeRoom(IRoom room, float? x = null, float? y = null);
	}
}

