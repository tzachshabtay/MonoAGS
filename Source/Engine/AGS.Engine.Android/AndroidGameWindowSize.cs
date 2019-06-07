using Android.Content.Res;
using AGS.Engine.Desktop;
using AGS.API;
using OpenTK;

namespace AGS.Engine.Android
{
	public class AndroidGameWindowSize : IGameWindowSize
	{
        private readonly float _density;

        public AndroidGameWindowSize()
        {
            _density = Resources.System.DisplayMetrics.Density;
        }

		#region IGameWindowSize implementation

        public int GetWidth(int width)
		{
            return convertPixelsToDp(width);
		}

		public int GetHeight(int height)
		{
            return convertPixelsToDp(height);
		}

        public bool AllowSetSize => false;

        public Rectangle GetWindow(Rectangle gameWindow)
        {
            return new Rectangle(0, 0, (int)(GetWidth(gameWindow.Width) - ((GLUtils.ScreenViewport.X * 2) / _density)), 
                                       (int)(GetHeight(gameWindow.Height) - ((GLUtils.ScreenViewport.Y * 2) / _density)));
        }

		#endregion

		private int convertPixelsToDp(float pixelValue)
		{
			var dp = (int) ((pixelValue)/Resources.System.DisplayMetrics.Density);
			return dp;
		}
	}
}