using System;
using Android.Content.Res;
using Android.Content;
using Android.App;
using Android.Runtime;
using Android.Views;

namespace AGS.Engine.Android
{
	public class AndroidGameWindowSize : IGameWindowSize
	{
		#region IGameWindowSize implementation

		public int GetWidth(OpenTK.GameWindow gameWindow)
		{
			var metrics = Resources.System.DisplayMetrics;
			return convertPixelsToDp(metrics.WidthPixels);
		}

		public int GetHeight(OpenTK.GameWindow gameWindow)
		{
			var metrics = Resources.System.DisplayMetrics;
			return convertPixelsToDp(metrics.HeightPixels);
		}

		public void SetSize(OpenTK.GameWindow gameWindow, AGS.API.Size size)
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

