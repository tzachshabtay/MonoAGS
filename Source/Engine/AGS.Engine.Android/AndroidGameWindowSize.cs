using Android.Content.Res;
using AGS.Engine.Desktop;
using OpenTK.Platform.Android;

namespace AGS.Engine.Android
{
	public class AndroidGameWindowSize : IGameWindowSize
	{
        private AndroidGameView _view;

        public AndroidGameWindowSize(AndroidGameView view)
        {
            _view = view;
        }

		#region IGameWindowSize implementation

        public int GetWidth(OpenTK.INativeWindow gameWindow)
		{
            return convertPixelsToDp(_view.Width);
		}

		public int GetHeight(OpenTK.INativeWindow gameWindow)
		{
            return convertPixelsToDp(_view.Height);
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

