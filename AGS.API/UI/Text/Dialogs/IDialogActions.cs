using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AGS.API
{
	public interface IDialogActions
	{
		IList<IDialogAction> Actions { get; }

		void AddText(ICharacter character, params string[] sentences);
		void AddPlayerText(params string[] sentences);
		void AddActions(params Action[] actions);
		void AddConditionalActions(params Func<bool>[] actions);

		void AddAsyncActions(params Func<Task>[] actions);
		void AddAsyncConditionalActions(params Func<Task<bool>>[] actions);

		void AddActions(params IDialogAction[] actions);

		Task<bool> RunAsync();
	}
}

