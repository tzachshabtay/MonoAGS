namespace AGS.API
{
    public interface IRoomFactory
	{
		IEdge GetEdge(float value = 0f);
		IRoom GetRoom(string id, float leftEdge = 0f, float rightEdge = 0f, float bottomEdge = 0f, float topEdge = 0f);
	}
}

