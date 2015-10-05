using System;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class BemanDialogs
	{
		public BemanDialogs()
		{
		}

		public IDialog StartDialog { get; private set; }

		public void Load(IGameFactory factory)
		{
			StartDialog = factory.GetDialog();
			createStartDialog(factory, createQuestionsDialog(factory));
		}

		private void createStartDialog(IGameFactory factory, IDialog questionsDialog)
		{
			StartDialog.StartupActions.AddPlayerText("Hello there!");
			StartDialog.StartupActions.AddText(Characters.Beman, "Hello yourself!");
			StartDialog.StartupActions.AddConditionalActions(() => Repeat.OnceOnly("BemanStartDialog"));
			StartDialog.StartupActions.AddText(Characters.Beman, "God, that's a relief.", "It's good to see I'm not alone in this place.");

			IDialogOption option1 = factory.GetDialogOption("Who are you?", showOnce: true);
			option1.AddText(Characters.Beman, "I am Beman, and you are?");
			option1.AddPlayerText("I am Cris.");

			IDialogOption option2 = factory.GetDialogOption("What is this place?");
			option2.AddText(Characters.Beman, "I have no idea. I just woke up here.");
			option2.AddPlayerText("Wow, seems like we share a similar story.");

			IDialogOption option3 = factory.GetDialogOption("Tell me a little bit about yourself.", speakOption: false);
			option3.AddText(Characters.Beman, "What do you want to know?");
			option3.ChangeDialogWhenFinished = questionsDialog;

			IDialogOption option4 = factory.GetDialogOption("I'll be going now.");
			option4.AddText(Characters.Beman, "Ok, see you around.");
			option4.ExitDialogWhenFinished = true;

			StartDialog.AddOptions(option1, option2, option3, option4);
		}

		private IDialog createQuestionsDialog(IGameFactory factory)
		{
			IDialogOption option1 = factory.GetDialogOption("Where are you from?");
			option1.AddText(Characters.Beman, "I'm from Sweden.");

			IDialogOption option2 = factory.GetDialogOption("What do you do?");
			option2.AddText(Characters.Beman, "I'm a hobbyist game developer.");

			IDialogOption option3 = factory.GetDialogOption("That's all I have...");
			option3.ChangeDialogWhenFinished = StartDialog;

			IDialog dialog = factory.GetDialog();
			dialog.AddOptions(option1, option2, option3);

			return dialog;
		}
	}
}

