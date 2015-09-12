using System;
using System.Threading.Tasks;

namespace API
{
	public interface ISaveLoad
	{
		void Save(string saveName);
		Task SaveAsync(string saveName);

		void Load(string saveName);
		Task LoadAsync(string saveName);
	}
}

