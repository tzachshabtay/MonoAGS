namespace AGS.API
{
    public interface IMaskLoader
	{
		IMask Load(string path, bool transparentMeansMasked = false, IColor debugDrawColor = null, 
			string saveMaskToFile = null, string id = null);

		IMask Load(IBitmap image, bool transparentMeansMasked = false, IColor debugDrawColor = null, 
			string saveMaskToFile = null);
	}
}

