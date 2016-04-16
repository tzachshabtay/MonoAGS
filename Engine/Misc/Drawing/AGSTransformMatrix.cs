using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSTransformMatrix : ITransformMatrix
	{
		public AGSTransformMatrix(float[] elements)
		{
			Elements = elements;
		}

		#region ITransformMatrix implementation

		public float[] Elements { get; private set; }

		#endregion
	}
}

