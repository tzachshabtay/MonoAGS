using System;
using Android.Content.Res;
using Android.Content;
using Android.App;
using Android.Runtime;
using Android.Views;
using AGS.Engine.Desktop;

namespace AGS.Engine.Android
{
	public class AndroidGameWindowSize : IGameWindowSize
	{
		#region IGameWindowSize implementation

        public int GetWidth(OpenTK.INativeWindow gameWindow)
		{
			var metrics = Resources.System.DisplayMetrics;
			return convertPixelsToDp(metrics.WidthPixels);
		}

		public int GetHeight(OpenTK.INativeWindow gameWindow)
		{
			var metrics = Resources.System.DisplayMetrics;
			return convertPixelsToDp(metrics.HeightPixels);
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

