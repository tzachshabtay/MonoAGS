using AGS.API;

namespace AGS.Engine
{
	public class AGSCutscene : ICutscene
	{
	    public AGSCutscene(IInput input)
		{
			input.KeyUp?.Subscribe(onKeyUp);
            input.MouseUp?.Subscribe(onMouseUp);
		}

		#region ICutscene implementation

        public SkipCutsceneTrigger SkipTrigger { get; set; }

		public void Start()
		{
			IsSkipping = false;
			IsRunning = true;
		}

		public bool End()
		{
            bool wasSkipping = IsSkipping;
			IsSkipping = false;
			IsRunning = false;
            return wasSkipping;
		}

        public void BeginSkip()
        {
            if (!IsRunning || IsSkipping) return;
            IsSkipping = true;
        }

		public void CopyFrom(ICutscene cutscene)
		{
			IsSkipping = cutscene.IsSkipping;
			IsRunning = cutscene.IsRunning;
		}

		public bool IsSkipping { get; private set; }
		public bool IsRunning { get; private set; }

		#endregion

		private void onKeyUp(KeyboardEventArgs args)
		{
            switch (SkipTrigger)
            {
                case SkipCutsceneTrigger.EscapeKeyOnly:
                    if (args.Key != Key.Escape) return;
                    break;
                case SkipCutsceneTrigger.Custom: return;
            }
            BeginSkip();
		}

        private void onMouseUp(MouseButtonEventArgs args)
        {
            if (SkipTrigger != SkipCutsceneTrigger.AnyKeyOrMouse) return;
            BeginSkip();
        }
	}
}

