using System;
using System.Drawing;

namespace AGS.API
{
	public interface IMaskLoader
	{
		IMask Load(string path, bool transparentMeansMasked = false, Color? debugDrawColor = null, 
			string saveMaskToFile = null);

		IMask Load(Bitmap image, bool transparentMeansMasked = false, Color? debugDrawColor = null, 
			string saveMaskToFile = null);
	}
}

