using System;
using AGS.API;
using AGS.Engine;
using System.Collections.Generic;
using DemoQuest;

namespace DemoGame
{
	public class BemanDialogs
	{
		private IGame _game;

		public BemanDialogs()
		{
		}

		public IDialog StartDialog { get; private set; }

		public void Load(IGame game)
		{
			_game = game;
			_game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSavedGameLoaded());
			initDialogs();
		}

		private void onSavedGameLoaded()
		{
			clearDialogs();
			initDialogs();
		}

		private void initDialogs()
		{
			IGameFactory factory = _game.Factory;
			StartDialog = factory.Dialog.GetDialog("Dialog: Beman- Start");
			createStartDialog(factory, createQuestionsDialog(factory), createShadersDialog(factory));
		}

		private void clearDialogs()
		{
			HashSet<string> ids = new HashSet<string> ();
			getDialogGraphics(StartDialog, ids);
			_game.State.UI.RemoveAll(o => ids.Contains(o.ID));
		}

		private void getDialogGraphics(IDialog dialog, HashSet<string> ids)
		{
			if (dialog == null) return;
			if (!ids.Add(dialog.Graphics.ID)) return;
			foreach (var option in dialog.Options)
			{
				option.Dispose();
				ids.Add(option.Label.ID);
				getDialogGraphics(option.ChangeDialogWhenFinished, ids);
			}
		}

		private void createStartDialog(IGameFactory factory, IDialog questionsDialog, IDialog shadersDialog)
		{
			StartDialog.StartupActions.AddPlayerText("Hello there!");
			StartDialog.StartupActions.AddText(Characters.Beman, "Hello yourself!");
			StartDialog.StartupActions.AddConditionalActions(() => Repeat.OnceOnly("BemanStartDialog"));
			StartDialog.StartupActions.AddText(Characters.Beman, "God, that's a relief.", "It's good to see I'm not alone in this place.");

			IDialogOption option1 = factory.Dialog.GetDialogOption("Who are you?", showOnce: true);
			option1.AddText(Characters.Beman, "I am Beman, and you are?");
			option1.AddPlayerText("I am Cris.");

			IDialogOption option2 = factory.Dialog.GetDialogOption("What is this place?");
			option2.AddText(Characters.Beman, "I have no idea. I just woke up here.");
			option2.AddPlayerText("Wow, seems like we share a similar story.");

			IDialogOption option3 = factory.Dialog.GetDialogOption("Tell me a little bit about yourself.", speakOption: false);
			option3.AddText(Characters.Beman, "What do you want to know?");
			option3.ChangeDialogWhenFinished = questionsDialog;

			IDialogOption option4 = factory.Dialog.GetDialogOption("Can I set a shader?");
			option4.AddText(Characters.Beman, "Sure, choose a shader...");
			option4.ChangeDialogWhenFinished = shadersDialog;

			IDialogOption option5 = factory.Dialog.GetDialogOption("I'll be going now.");
			option5.AddText(Characters.Beman, "Ok, see you around.");
			option5.ExitDialogWhenFinished = true;

			StartDialog.AddOptions(option1, option2, option3, option4, option5);
		}

		private IDialog createQuestionsDialog(IGameFactory factory)
		{
			IDialogOption option1 = factory.Dialog.GetDialogOption("Where are you from?");
			option1.AddText(Characters.Beman, "I'm from Sweden.");

			IDialogOption option2 = factory.Dialog.GetDialogOption("What do you do?");
			option2.AddText(Characters.Beman, "I'm a hobbyist game developer.");

			IDialogOption option3 = factory.Dialog.GetDialogOption("Can I start a scene?");
			option3.ExitDialogWhenFinished = true;
			option3.AddText(Characters.Beman, "Go for it, though remember that the user can skip the scene by pressing any key on the keyboard");
			option3.AddConditionalActions(startAScene);

			IDialogOption option4 = factory.Dialog.GetDialogOption("That's all I have...");
			option4.ChangeDialogWhenFinished = StartDialog;

			IDialog dialog = factory.Dialog.GetDialog("Dialog: Beman- Questions");
			dialog.AddOptions(option1, option2, option3, option4);

			return dialog;
		}

		private IDialog createShadersDialog(IGameFactory factory)
		{
			IDialogOption option1 = factory.Dialog.GetDialogOption("Normal");
			IDialogOption option2 = factory.Dialog.GetDialogOption("Grayscale");
			IDialogOption option3 = factory.Dialog.GetDialogOption("Sepia");
			IDialogOption option4 = factory.Dialog.GetDialogOption("Soft Sepia");
			IDialogOption option5 = factory.Dialog.GetDialogOption("Vignette");
			IDialogOption option6 = factory.Dialog.GetDialogOption("Blur me!");
			IDialogOption option7 = factory.Dialog.GetDialogOption("Actually, I don't want a shader!");

			setShaderOption(option1, () => Shaders.SetStandardShader());
			setShaderOption(option2, () => Shaders.SetGrayscaleShader());
			setShaderOption(option3, () => Shaders.SetSepiaShader());
			setShaderOption(option4, () => Shaders.SetSoftSepiaShader());
			setShaderOption(option5, () => Shaders.SetVignetteShader());
			setShaderOption(option6, () => Shaders.SetBlurShader());
			setShaderOption(option7, () => Shaders.TurnOffShader());

			IDialog dialog = factory.Dialog.GetDialog("Dialog: Beman- Shaders");
			dialog.AddOptions(option1, option2, option3, option4, option5, option6, option7);

			return dialog;
		}

		private void setShaderOption(IDialogOption option, Action setShader)
		{
			option.AddText(Characters.Beman, "Your wish is my command.");
			option.AddActions(setShader);
			option.ExitDialogWhenFinished = true;
		}

		private bool startAScene()
		{
			ICharacter player = _game.State.Player.Character;
			_game.State.Cutscene.Start();

			player.Say("Scene is now in session.");
			player.Walk(new AGSLocation (0f, player.Y));
			player.ChangeRoom(Rooms.EmptyStreet);
			player.Say("This scene involves switching rooms!");
			player.Walk(new AGSLocation (250f, player.Y));

			_game.State.Cutscene.End();

			player.Say("End scene.");
			return true;
		}
	}
}

