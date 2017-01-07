using Android.Content.Res;
using AGS.Engine.Desktop;

namespace AGS.Engine.Android
{
	public class AndroidGameWindowSize : IGameWindowSize
	{
        private readonly IGameWindow _gameWindow;

        public AndroidGameWindowSize(IGameWindow gameWindow)
        {
            _gameWindow = gameWindow;
        }

		#region IGameWindowSize implementation

        public int GetWidth(OpenTK.INativeWindow gameWindow)
		{
            return convertPixelsToDp(_gameWindow.Width);
		}

		public int GetHeight(OpenTK.INativeWindow gameWindow)
		{
            return convertPixelsToDp(_gameWindow.Height);
		}

		public void SetSize(OpenTK.INativeWindow gameWindow, AGS.API.Size size)
		{
		}

		#endregion

		private int convertPixelsToDp(float pixelValue)
		{
			var dp = (int) ((pixelValue)/Resources.System.DisplayMetrics.Density);
			return dp;
		}
	}
}

