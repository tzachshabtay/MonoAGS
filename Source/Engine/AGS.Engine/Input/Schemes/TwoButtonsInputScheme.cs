using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AGS.Engine
{
	public class TwoButtonsInputScheme
	{
		private readonly IGame _game;
        private int _handlingClick;
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
            if (Interlocked.CompareExchange(ref _handlingClick, 1, 0) != 0) return;
            try
            {
                var state = _game.State;
                var hitTest = _game.HitTest;
                if (!state.Player.Enabled) return;

                if (e.Button == MouseButton.Left)
                {
                    await onLeftMouseDown(e, state, hitTest);
                }
                else if (e.Button == MouseButton.Right)
                {
                    await onRightMouseDown(e, state, hitTest);
                }
            }
            finally
            {
                _handlingClick = 0;
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    state.Player.WalkAsync(position);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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

                await hotComp.Interactions.OnInteract(AGSInteractions.INTERACT).InvokeAsync(new ObjectEventArgs(hotspot));
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

            await hotComp.Interactions.OnInteract(AGSInteractions.LOOK).InvokeAsync(new ObjectEventArgs(hotspot));
        }
    }
}