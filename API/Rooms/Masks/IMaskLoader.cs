using System;
using System.Drawing;

namespace API
{
	public interface IMaskLoader
	{
		IMask Load(string path, bool transparentMeansMasked = false, Color? debugDrawColor = null, 
			string saveMaskToFile = null);
	}
}

