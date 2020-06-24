﻿using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSAnimationConfiguration : IAnimationConfiguration
	{
		public AGSAnimationConfiguration ()
		{
            DelayBetweenFrames = 5;
		}

		#region IAnimationConfiguration implementation

		public LoopingStyle Looping { get; set; }

		public int Loops { get; set; }

        public int DelayBetweenFrames { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        #endregion

        public override string ToString()
        {
            if (Loops == 0) return $"Repeating animation, looping: {Looping}, delay: {DelayBetweenFrames}";
            return $"{Loops} loop(s), looping: {Looping}, delay: {DelayBetweenFrames}";
        }
    }
}