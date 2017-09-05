using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class TwoButtonsInputScheme
	{
		private IGame _game;
		private IInputEvents _input;

		public TwoButtonsInputScheme (IGame game, IInputEvents input)
		{
            this._game = game;
			this._input = input;
		}

		public void Start()
		{
			_input.MouseDown.SubscribeToAsync(onMouseDown);
		}
			
		private async Task onMouseDown(MouseButtonEventArgs e)
		{
            IGameState state = _game.State;
			if (!state.Player.Enabled)
				return;

			if (e.Button == MouseButton.Left)
			{
				if (state.Player.Inventory == null || 
					state.Player.Inventory.ActiveItem == null)
				{
                    Vector2 xy = e.MousePosition.GetProjectedPoint(state.Viewport, state.Player);
                    AGSLocation location = new AGSLocation(xy.X, xy.Y, state.Player.Z);
					await state.Player.WalkAsync(location).ConfigureAwait(true);
				}
				else
				{

				}
			}
			else if (e.Button == MouseButton.Right)
			{
				IInventory inventory = state.Player.Inventory;
				if (inventory == null) return;
				if (inventory.ActiveItem == null)
				{
                    IObject hotspot = _game.HitTest.ObjectAtMousePosition;
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

