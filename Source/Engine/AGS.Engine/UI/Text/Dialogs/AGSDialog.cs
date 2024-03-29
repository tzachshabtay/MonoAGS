﻿using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
	public class AGSDialog : IDialog
	{
		private readonly IDialogLayout _dialogLayout;
		private bool _inDialog;

		public AGSDialog(IDialogLayout layout, IDialogActions startupActions, IObject graphics, bool showWhileOptionsAreRunning = false)
		{
			_dialogLayout = layout;
			StartupActions = startupActions;
			Graphics = graphics;
			ShowWhileOptionsAreRunning = showWhileOptionsAreRunning;
			Options = new List<IDialogOption> (5);

			foreach (var option in Options)
			{
				option.Label.TreeNode.SetParent(Graphics.TreeNode);
			}
			Graphics.Visible = false;
		}

		#region IDialog implementation

		public async Task RunAsync()
		{
			if (!_inDialog)
			{
				await StartupActions.RunAsync();
			}
			_inDialog = true;
			await dialogLoop();
			_inDialog = false;
		}

		public void AddOptions(params IDialogOption[] options)
		{
			foreach (var option in options)
			{
				Options.Add(option);
				option.Label.TreeNode.SetParent(Graphics.TreeNode);
			}
		}

		public IObject Graphics { get; private set; }

		public IList<IDialogOption> Options { get; private set; }

		public bool ShowWhileOptionsAreRunning { get; private set; }


		public IDialogActions StartupActions { get; private set; }

		#endregion

		private async Task dialogLoop()
		{
			TaskCompletionSource<IDialogOption> selectedOptionTask = new TaskCompletionSource<IDialogOption> (null);
			List<Action<MouseButtonEventArgs>> callbacks = 
				new List<Action<MouseButtonEventArgs>> (Options.Count);
			foreach (var option in Options)
			{
				Action<MouseButtonEventArgs> callback = _ => selectedOptionTask.TrySetResult(option);
				callbacks.Add(callback);
				option.Label.MouseClicked.Subscribe(callback);
			}
			Graphics.Visible = true;
			await _dialogLayout.LayoutAsync(Graphics, Options);
			if (!Options.Any(o => o.Label.Visible))
			{
				Graphics.Visible = false;
				return;
			}

			IDialogOption selectedOption = await selectedOptionTask.Task;
			for (int index = 0; index < Options.Count; index++)
			{
				Options[index].Label.MouseClicked.Unsubscribe(callbacks[index]);
			}
			if (!ShowWhileOptionsAreRunning)
			{
				Graphics.Visible = false;
			}
			await selectedOption.RunAsync();
			await Task.Delay(300); //This delay is to avoid player clicks for skipping speech to accidentally trigger a new dialog option
			Graphics.Visible = !selectedOption.ExitDialogWhenFinished && selectedOption.ChangeDialogWhenFinished == null;

			if (selectedOption.ChangeDialogWhenFinished != null)
			{
				await selectedOption.ChangeDialogWhenFinished.RunAsync();
			}
			else if (!selectedOption.ExitDialogWhenFinished)
			{
				await dialogLoop();
			}
		}
	}
}

