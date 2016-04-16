using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
	public interface IGameWindowSize
	{
		int GetWidth(GameWindow gameWindow);
		int GetHeight(GameWindow gameWindow);
		void SetSize(GameWindow gameWindow, Size size);
	}
}

