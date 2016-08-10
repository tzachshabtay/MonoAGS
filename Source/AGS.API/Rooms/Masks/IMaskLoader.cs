using System.Threading.Tasks;

namespace AGS.API
{
    public interface IMaskLoader
	{
		IMask Load(string path, bool transparentMeansMasked = false, Color? debugDrawColor = null, 
			string saveMaskToFile = null, string id = null);

		Task<IMask> LoadAsync (string path, bool transparentMeansMasked = false,
		    Color? debugDrawColor = null, string saveMaskToFile = null, string id = null);

		IMask Load(IBitmap image, bool transparentMeansMasked = false, Color? debugDrawColor = null, 
			string saveMaskToFile = null);
	}
}

