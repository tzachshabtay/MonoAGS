using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSCutscene : ICutscene
	{
		private readonly IInput _input;

		public AGSCutscene(IInput input)
		{
			_input = input;
			_input.KeyUp.Subscribe(onKeyUp);
		}

		#region ICutscene implementation

		public void Start()
		{
			IsSkipping = false;
			IsRunning = true;
		}

		public void End()
		{
			IsSkipping = false;
			IsRunning = false;
		}

		public bool IsSkipping { get; private set; }
		public bool IsRunning { get; private set; }

		#endregion

		private void onKeyUp(object sender, KeyboardEventArgs args)
		{
			if (!IsRunning || IsSkipping) return;
			IsSkipping = true;
		}
	}
}

