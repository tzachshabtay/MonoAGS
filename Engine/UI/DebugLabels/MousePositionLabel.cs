using System;
using AGS.API;

namespace AGS.Engine
{
	public class MousePositionLabel
	{
		private ILabel _label;
		private IInputEvents _inputEvents;

		public MousePositionLabel(IInputEvents events, ILabel label)
		{
			_label = label;
			_inputEvents = events;
		}

		public void Start()
		{
			_inputEvents.MouseMove.Subscribe(onMouseMove);
		}

		private void onMouseMove(object sender, MousePositionEventArgs args)
		{
			_label.Text = new AGSPoint (args.X, args.Y).ToString();
		}
	}
}

