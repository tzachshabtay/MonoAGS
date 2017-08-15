using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSDialogOption : IDialogOption
	{
		private readonly ITextConfig _normalConfig;
		private readonly IDialogActions _actions;
        private readonly ICharacter _player;
        private bool _hasBeenChosen;

		//Parameter names for speakOption and showOnce are used in the factory, changing the names requires factory code change as well
		public AGSDialogOption(IDialogActions actions, ICharacter player, ILabel label, bool exitDialogOnFinish = false, 
                               bool speakOption = true, bool showOnce = false, ITextConfig hoverConfig = null, ITextConfig hasBeenChosenConfig = null)
		{
			_actions = actions;
			_player = player;
			Label = label;
			_normalConfig = label.TextConfig;
			HoverConfig = hoverConfig;
            HasBeenChosenConfig = hasBeenChosenConfig;
			ExitDialogWhenFinished = exitDialogOnFinish;
			SpeakOption = speakOption;
			ShowOnce = showOnce;
			label.MouseEnter.Subscribe(onMouseEnter);
			label.MouseLeave.Subscribe(onMouseLeave);
		}

		#region IDialogOption implementation

		public void Run()
		{
			Task.Run(async() => await RunAsync()).Wait();
		}

		public async Task<bool> RunAsync()
		{
			if (SpeakOption)
			{
				await _player.SayAsync(Label.Text);
			}
			if (await _actions.RunAsync() && ShowOnce)
			{
				Label.Visible = false;
			}
            HasOptionBeenChosen = true;
			return true;
		}

		public void AddText(ICharacter character, params string[] sentences)
		{
			_actions.AddText(character, sentences);
		}

		public void AddPlayerText(params string[] sentences)
		{
			_actions.AddPlayerText(sentences);
		}

		public void AddActions(params Action[] actions)
		{
			_actions.AddActions(actions);
		}

		public void AddConditionalActions(params Func<bool>[] actions)
		{
			_actions.AddConditionalActions(actions);
		}

		public void AddAsyncActions(params Func<Task>[] actions)
		{
			_actions.AddAsyncActions(actions);
		}

		public void AddAsyncConditionalActions(params Func<Task<bool>>[] actions)
		{
			_actions.AddAsyncConditionalActions(actions);
		}

		public void AddActions(params IDialogAction[] actions)
		{
			_actions.AddActions(actions);
		}

		public void Dispose()
		{
			Label.Dispose();
			Label.MouseEnter.Unsubscribe(onMouseEnter);
			Label.MouseLeave.Unsubscribe(onMouseLeave);
		}

		public ILabel Label { get; private set; }

		public ITextConfig HoverConfig { get; private set; }

        public ITextConfig HasBeenChosenConfig { get; private set; }

		public bool SpeakOption { get; private set; }

		public bool ShowOnce { get; private set; }

		public bool ExitDialogWhenFinished { get; set; }

		public IDialog ChangeDialogWhenFinished { get; set; }

		public IList<IDialogAction> Actions { get { return _actions.Actions; } }

        public bool HasOptionBeenChosen 
        { 
            get { return _hasBeenChosen; } 
            set 
            { 
                _hasBeenChosen = value; 
                Label.TextConfig = getNormalTextConfig();
            } 
        }

		#endregion

		private void onMouseEnter(MousePositionEventArgs e)
		{
			Label.TextConfig = HoverConfig ?? getNormalTextConfig();
		}

		private void onMouseLeave(MousePositionEventArgs e)
		{
			Label.TextConfig = getNormalTextConfig();
		}

        private ITextConfig getNormalTextConfig()
        {
            return _hasBeenChosen ? HasBeenChosenConfig ?? _normalConfig : _normalConfig;
        }
	}
}

