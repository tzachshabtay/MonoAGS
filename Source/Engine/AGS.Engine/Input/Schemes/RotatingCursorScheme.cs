using AGS.API;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AGS.Engine
{
	public class RotatingCursorScheme
	{
		private List<Cursor> _cursors = new List<Cursor>(10);
		private readonly IGame _game;
		private int _handlingClick;

		public const string WALK_MODE = "Walk";
		public const string LOOK_MODE = "Look";
		public const string INTERACT_MODE = "Interact";
		public const string WAIT_MODE = "Wait";
		public const string INVENTORY_MODE = "Inventory";

		private int _currentMode;
		private Cursor _inventoryCursor;

		public RotatingCursorScheme(IGame game, IObject lookCursor, IObject walkCursor,
            IObject interactCursor, IObject waitCursor)
		{
			RotatingEnabled = true;
            Trace.Assert(game != null, "Please pass in a valid game argument");
            Trace.Assert(game.Events != null, "Game events passed as argument is null, please make sure to only pass the input after the game is loaded (for example, in the game's OnLoad event");
            _game = game;
			_game.Events.OnSavedGameLoad.Subscribe(onSavedGameLoad);

			if (walkCursor != null) AddCursor(WALK_MODE, walkCursor, true);
			if (lookCursor != null) AddCursor(LOOK_MODE, lookCursor, true);
			if (interactCursor != null) AddCursor(INTERACT_MODE, interactCursor, true);
			if (waitCursor != null) AddCursor(WAIT_MODE, waitCursor, false);
			AddCursor(INVENTORY_MODE, null, true);
			_inventoryCursor = _cursors[_cursors.Count - 1];
		}

		public bool RotatingEnabled { get; set; }

		public string CurrentMode 
		{
            get => _cursors[_currentMode].Mode;
            set 
			{
				int index = _cursors.FindIndex(c => c.Mode == value);
				if (index < 0)
				{
					Debug.WriteLine($"Did not find cursor mode {value}");
					return;
				}
				_currentMode = index;
				setCursor();
			}
		}
			
		public void AddCursor(string mode, IObject animation, bool rotating)
		{
			_cursors.Add(new Cursor (mode, animation, rotating));
		}

		public void Start()
		{
            Trace.Assert(_game.Input != null, "Game input passed as argument is null, please make sure to only start the control scheme after the game is loaded (for example, in the game's OnLoad event");
            setCursor();
			_game.Input.MouseDown.SubscribeToAsync(onMouseDown);
		}

		public void SetInventoryCursor()
		{
			_inventoryCursor.Animation = _game.State.Player.Inventory.ActiveItem.CursorGraphics;
			CurrentMode = INVENTORY_MODE;
		}

		private void setCursor()
		{
			_game.State.Cursor = _cursors[_currentMode].Animation;
		}

		private void onSavedGameLoad()
		{
			_currentMode = 0;
			setCursor();
			RotatingEnabled = true;
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
    				onRightMouseDown(e, state);
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
			string mode = CurrentMode;
            IObject hotspot = hitTest.ObjectAtMousePosition;
            IHotspotComponent hotComp = hotspot?.GetComponent<IHotspotComponent>();

            if (_game.State.Cursor != _inventoryCursor.Animation)
			{
				if (hotspot != null && hotspot.Room == null) 
				{
					IInventoryItem inventoryItem = state.Player.Inventory.Items.FirstOrDefault(
						i => i.Graphics == hotspot);
					if (inventoryItem != null)
					{
						if (mode != LOOK_MODE)
						{
							if (inventoryItem.ShouldInteract) mode = INTERACT_MODE;
							else
							{
								state.Player.Inventory.ActiveItem = inventoryItem;
								SetInventoryCursor();
								return;
							}
						}
					}
					else return; //Blocking clicks when hovering UI objects
				}

				if (mode == WALK_MODE)
				{
                    var xy = e.MousePosition.GetProjectedPoint(state.Viewport, state.Player);
                    Position position = new Position(xy.X, xy.Y, state.Player.Z);
					await state.Player.WalkAsync(position).ConfigureAwait(true);
				}
				else if (mode != WAIT_MODE)
				{
					if (hotComp == null) return;

					if (mode == LOOK_MODE)
					{
                        await hotComp.Interactions.OnInteract(AGSInteractions.LOOK).InvokeAsync(new ObjectEventArgs (hotspot));
					}
					else if (mode == INTERACT_MODE)
					{
                        await hotComp.Interactions.OnInteract(AGSInteractions.INTERACT).InvokeAsync(new ObjectEventArgs (hotspot));
					}
					else
					{
                        await hotComp.Interactions.OnInteract(mode).InvokeAsync(new ObjectEventArgs (hotspot));
					}
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

		private void onRightMouseDown(MouseButtonEventArgs e, IGameState state)
		{
			if (!RotatingEnabled) return;

			IInventory inventory = state.Player.Inventory;

			int startMode = _currentMode;
			Cursor cursor = _cursors[_currentMode];

			if (!cursor.Rotating) return;

			do
			{
				_currentMode = (_currentMode + 1) % _cursors.Count;
				cursor = _cursors[_currentMode];
			} while ((!cursor.Rotating || cursor.Animation == null) && _currentMode != startMode);

			setCursor();
		}

		private class Cursor
		{
			public Cursor(string mode, IObject animation, bool rotating)
			{
				Mode = mode;
				Animation = animation;
				Rotating = rotating;
			}

			public string Mode { get; private set; }
			public IObject Animation { get; set; }
			public bool Rotating { get; private set; }
		}
	}
}