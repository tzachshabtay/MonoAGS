namespace AGS.API
{
    public interface IImageRenderer
	{
		void Prepare(IObject obj, IViewport viewport, IPoint areaScaling);
		void Render(IObject obj, IViewport viewport, IPoint areaScaling);
	}
}

