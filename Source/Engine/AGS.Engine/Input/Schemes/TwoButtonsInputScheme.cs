using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class TwoButtonsInputScheme
	{
		private IGameState _state;
		private IInputEvents _input;

		public TwoButtonsInputScheme (IGameState state, IInputEvents input)
		{
			this._state = state;
			this._input = input;
		}

		public void Start()
		{
			_input.MouseDown.SubscribeToAsync(onMouseDown);
		}
			
		private async Task onMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!_state.Player.Character.Enabled)
				return;

			if (e.Button == MouseButton.Left)
			{
				if (_state.Player.Character.Inventory == null || 
					_state.Player.Character.Inventory.ActiveItem == null)
				{
					AGSLocation location = new AGSLocation(e.X, e.Y, _state.Player.Character.Z);
					await _state.Player.Character.WalkAsync(location).ConfigureAwait(true);
				}
				else
				{

				}
			}
			else if (e.Button == MouseButton.Right)
			{
				IInventory inventory = _state.Player.Character.Inventory;
				if (inventory == null) return;
				if (inventory.ActiveItem == null)
				{
					IObject hotspot = _state.Room.GetObjectAt(e.X, e.Y);
					if (hotspot == null) return;
				}
				else
				{
					inventory.ActiveItem = null;
				}
			}
		}
	}
}

