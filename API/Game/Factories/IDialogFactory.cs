using System;

namespace AGS.API
{
	public interface IDialogFactory
	{
		IDialogOption GetDialogOption(string text, ITextConfig config = null, ITextConfig hoverConfig = null,
			bool speakOption = true, bool showOnce = false);
		IDialog GetDialog(float x = 0f, float y = 0f, IObject graphics = null, 
			bool showWhileOptionsAreRunning = false, params IDialogOption[] options);
	}
}

