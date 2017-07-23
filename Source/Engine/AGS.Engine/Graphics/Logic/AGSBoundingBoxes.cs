﻿using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSBoundingBoxes
	{
		public AGSBoundingBoxes()
		{
            RenderBox = new AGSBoundingBox();
            HitTestBox = new AGSBoundingBox();
		}

		#region AGSBoundingBoxes implementation

		public AGSBoundingBox RenderBox { get; set; }

		public AGSBoundingBox HitTestBox { get; set; }

		#endregion
	}
}

