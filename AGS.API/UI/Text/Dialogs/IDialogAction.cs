using System.Threading.Tasks;

namespace AGS.API
{
    public interface IDialogAction
	{
		bool Enabled { get; set; }
		Task<bool> RunActionAsync();
	}
}

