using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace AGS.Engine
{
	public class TwoButtonsInputScheme
	{
		private readonly IGame _game;
        private bool _handlingClick;
        private IObject _inventoryCursor;
        private IObject _previousCursor;

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
            var state = _game.State;
            var hitTest = _game.HitTest;
            if (_handlingClick || !state.Player.Enabled)
                return;

            if (e.Button == MouseButton.Left)
            {
                await onLeftMouseDown(e, state, hitTest);
            }
            else if (e.Button == MouseButton.Right)
            {
                await onRightMouseDown(e, state, hitTest);
            }
		}

        private async Task onLeftMouseDown(MouseButtonEventArgs e, IGameState state, IHitTest hitTest)
        {
            if (!e.MousePosition.InWindow) return;
            IObject hotspot = hitTest.ObjectAtMousePosition;
            IHotspotComponent hotComp = hotspot?.GetComponent<IHotspotComponent>();

            if (state.Cursor != _inventoryCursor || _inventoryCursor == null)
            {
                if (hotspot == null)
                {
                    var xy = e.MousePosition.GetProjectedPoint(state.Viewport, state.Player);
                    Position position = new Position(xy.X, xy.Y, state.Player.Z);
                    await state.Player.WalkAsync(position).ConfigureAwait(true);
                    return;
                }
                if (hotspot.Room == null)
                {
                    IInventoryItem inventoryItem = state.Player.Inventory.Items.FirstOrDefault(
                        i => i.Graphics == hotspot);
                    if (inventoryItem != null)
                    {
                        if (!inventoryItem.ShouldInteract)
                        {
                            state.Player.Inventory.ActiveItem = inventoryItem;
                            _inventoryCursor = inventoryItem.CursorGraphics ?? inventoryItem.Graphics;
                            _previousCursor = state.Cursor;
                            state.Cursor = _inventoryCursor;
                            return;
                        }
                    }
                    else
                        return; //Blocking clicks when hovering UI objects
                }

                if (hotComp == null) return;

                _handlingClick = true;
                try
                {
                    await hotComp.Interactions.OnInteract(AGSInteractions.INTERACT).InvokeAsync(new ObjectEventArgs(hotspot));
                }
                finally
                {
                    _handlingClick = false;
                }
            }
            else if (hotComp != null)
            {
                if (hotspot.Room == null)
                {
                    IInventoryItem inventoryItem = state.Player.Inventory.Items.FirstOrDefault(
                                                      i => i.Graphics == hotspot);
                    if (inventoryItem != null)
                    {
                        var activeItem = state.Player.Inventory.ActiveItem;
                        await activeItem.OnCombination(inventoryItem).InvokeAsync(new InventoryCombinationEventArgs(activeItem, inventoryItem));
                    }
                    return;
                }

                await hotComp.Interactions.OnInventoryInteract(AGSInteractions.INTERACT).InvokeAsync(new InventoryInteractEventArgs(hotspot,
                    state.Player.Inventory.ActiveItem));
            }
        }

        private async Task onRightMouseDown(MouseButtonEventArgs e, IGameState state, IHitTest hitTest)
        {
            if (state.Cursor == _inventoryCursor && _inventoryCursor != null)
            {
                state.Cursor = _previousCursor;
                state.Player.Inventory.ActiveItem = null;
                return;
            }

            if (!e.MousePosition.InWindow) return;

            IObject hotspot = hitTest.ObjectAtMousePosition;
            IHotspotComponent hotComp = hotspot?.GetComponent<IHotspotComponent>();

            if (hotComp == null) return;

            try
            {
                await hotComp.Interactions.OnInteract(AGSInteractions.LOOK).InvokeAsync(new ObjectEventArgs(hotspot));
            }
            finally
            {
                _handlingClick = false;
            }
        }
    }
}