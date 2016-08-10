using System;

namespace AGS.Engine.Desktop
{
	public class DesktopGameWindowSize : IGameWindowSize
	{
		#region IGameWindowSize implementation

		public int GetWidth(OpenTK.GameWindow gameWindow)
		{
			return gameWindow.ClientSize.Width;
		}

		public int GetHeight(OpenTK.GameWindow gameWindow)
		{
			return gameWindow.ClientSize.Height;
		}

		public void SetSize(OpenTK.GameWindow gameWindow, AGS.API.Size size)
		{
			gameWindow.Size = new System.Drawing.Size (size.Width, size.Height);
		}

		#endregion
	}
}

