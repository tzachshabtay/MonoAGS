using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSScrollbar : IScrollbar
    {
        public AGSScrollbar(IButton upButton, IButton downButton, ISlider slider)
        {
            UpButton = upButton;
            DownButton = downButton;
            Slider = slider;
        }

        public IButton UpButton { get; }

        public IButton DownButton { get; }

        public ISlider Slider { get; }

        public float Step { get; set; }
    }
}
