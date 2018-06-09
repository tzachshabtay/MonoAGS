using Android.Content.Res;
using AGS.Engine.Desktop;
using AGS.API;
using OpenTK;

namespace AGS.Engine.Android
{
	public class AndroidGameWindowSize : IGameWindowSize
	{
        private readonly IGameWindow _gameWindow;
        private readonly float _density;

        public AndroidGameWindowSize(IGameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _density = Resources.System.DisplayMetrics.Density;
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

        public Rectangle GetWindow(INativeWindow gameWindow)
        {
            return new Rectangle(0, 0, (int)(GetWidth(gameWindow) - ((GLUtils.ScreenViewport.X * 2) / _density)), 
                                       (int)(GetHeight(gameWindow) - ((GLUtils.ScreenViewport.Y * 2) / _density)));
        }

		#endregion

		private int convertPixelsToDp(float pixelValue)
		{
			var dp = (int) ((pixelValue)/Resources.System.DisplayMetrics.Density);
			return dp;
		}
	}
}