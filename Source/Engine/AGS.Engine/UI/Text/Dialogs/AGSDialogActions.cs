using System;
using AGS.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSDialogActions : IDialogActions
	{
        private ICharacter _player;

		public AGSDialogActions(ICharacter player)
		{
			_player = player;
			Actions = new List<IDialogAction> (5);
		}

		public IList<IDialogAction> Actions { get; private set; }

		public void AddText(ICharacter character, params string[] sentences)
		{
			foreach (string sentence in sentences)
			{
				Actions.Add(new AGSDialogAction (character, sentence));
			}
		}

		public void AddPlayerText(params string[] sentences)
		{
			foreach (string sentence in sentences)
			{
				Actions.Add(new AGSDialogAction (_player, sentence));
			}
		}

		public void AddActions(params Action[] actions)
		{
			foreach (var action in actions)
			{
				Actions.Add(new AGSDialogAction (action));
			}
		}

		public void AddConditionalActions(params Func<bool>[] actions)
		{
			foreach (var action in actions)
			{
				Actions.Add(new AGSDialogAction (action));
			}
		}

		public void AddAsyncActions(params Func<Task>[] actions)
		{
			foreach (var action in actions)
			{
				Actions.Add(new AGSDialogAction (action));
			}
		}

		public void AddAsyncConditionalActions(params Func<Task<bool>>[] actions)
		{
			foreach (var action in actions)
			{
				Actions.Add(new AGSDialogAction (action));
			}
		}

		public void AddActions(params IDialogAction[] actions)
		{
			foreach (var action in actions)
			{
				Actions.Add(action);
			}
		}

		public async Task<bool> RunAsync()
		{
			foreach (IDialogAction action in Actions)
			{
				if (!action.Enabled) continue;
				if (!await action.RunActionAsync())
				{
					return false;
				}
			}
			return true;
		}
	}
}

