using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGS.API
{
    public interface IDialog
	{
		IObject Graphics { get; }
		IList<IDialogOption> Options { get; }
		bool ShowWhileOptionsAreRunning { get; }

		IDialogActions StartupActions { get; }

		void AddOptions(params IDialogOption[] options);

		void Run();
		Task RunAsync();
	}
}

