using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSDialogAction : IDialogAction
	{
		private Func<Task<bool>> _action;

		public AGSDialogAction(Func<Task<bool>> action)
		{
			_action = action;
			Enabled = true;
		}

		public AGSDialogAction(Func<Task> action)
			:this(async () =>
			{
				await action();
				return true;
			})
		{
		}

		public AGSDialogAction(Func<bool> action)
			:this(() => Task.FromResult(action()))
		{
		}

		public AGSDialogAction(Action action)
			:this(() =>
			{
				action();
				return Task.FromResult(true);
			})
		{
		}

		public AGSDialogAction(ICharacter character, string text)
			:this(async () =>
			{
				if (character == null)
				{
					Debug.WriteLine("Null character received for dialog text: {0}", text);
					return true;
				}
				await character.SayAsync(text);
				return true;
			})
		{
		}

		#region IDialogAction implementation

		public async Task<bool> RunActionAsync()
		{
			if (!Enabled || _action == null) return true;
			return await _action();
		}

		public bool Enabled { get; set; }

		#endregion
	}
}

