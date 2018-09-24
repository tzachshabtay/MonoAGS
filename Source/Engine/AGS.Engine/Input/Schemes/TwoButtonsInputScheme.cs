using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AGS.Engine
{
	public class TwoButtonsInputScheme
	{
		private IGame _game;

		public TwoButtonsInputScheme (IGame game)
		{
            Trace.Assert(game != null, "Please pass in a valid game argument");
            this._game = game;
		}

		public void Start()
		{
            Trace.Assert(_game.Input != null, "Game input passed as argument is null, please make sure to only start the control scheme after the game is loaded (for example, in the game's OnLoad event");
            _game.Input.MouseDown.SubscribeToAsync(onMouseDown);
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
                    var xy = e.MousePosition.GetProjectedPoint(state.Viewport, state.Player);
                    Position position = new Position(xy.X, xy.Y, state.Player.Z);
					await state.Player.WalkAsync(position).ConfigureAwait(true);
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