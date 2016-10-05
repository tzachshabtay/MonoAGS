using System;

namespace AGS.API
{
    public interface IDialogOption : IDialogActions, IDisposable
	{
		ILabel Label { get; }
		ITextConfig HoverConfig { get; }
        ITextConfig HasBeenChosenConfig { get; }
		bool SpeakOption { get; }
		bool ShowOnce { get; }

		bool ExitDialogWhenFinished { get; set; }
		IDialog ChangeDialogWhenFinished { get; set; }

        bool HasOptionBeenChosen { get; set; }

		void Run();
	}
}

