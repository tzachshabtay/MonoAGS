﻿using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class VerbOnHotspotLabel
	{
		private readonly Func<string> _getMode;
		private readonly IGameEvents _events;
		private ILabel _label;
		private readonly IInput _input;
		private IGameState _state;
		private readonly IGame _game;
		private readonly Dictionary<string, string> _verbFormats;

		public VerbOnHotspotLabel(Func<string> getMode, IGame game, ILabel label)
		{
			_getMode = getMode;
			_label = label;
			_events = game.Events;
			_input = game.Input;
			_state = game.State;
			_game = game;

			_verbFormats = new Dictionary<string, string> (10)
			{
				{RotatingCursorScheme.LOOK_MODE, "Look on {0}"},
				{RotatingCursorScheme.INTERACT_MODE, "Interact with {0}"},
				{RotatingCursorScheme.WALK_MODE, "Walk to {0}"},
                {RotatingCursorScheme.INVENTORY_MODE, "Use {1} on {0}" },
				{"Talk", "Talk to {0}"},
			};
		}

		public void AddVerb(string verb, string format)
		{
			_verbFormats[verb] = format;
		}

		public void Start()
		{		
			_events.OnRepeatedlyExecute.Subscribe(onTick);
			_events.OnSavedGameLoad.Subscribe((sender, e) => onSavedGameLoaded());
		}

		private void onSavedGameLoaded()
		{
			ILabel oldLabel = _label;
			_state = _game.State;
			_label = _game.Find<ILabel>(oldLabel.ID);
		}

		private void onTick(object sender, EventArgs args)
		{
			if (_label == null) return;
			PointF position = _input.MousePosition;
			IObject obj = _state.Player.Character.Room.GetObjectAt (position.X, position.Y);
			if (obj == null || obj.Hotspot == null) 
			{
				_label.Visible = false;
				return;
			}
			_label.Visible = true;
            IInventoryItem inventoryItem = _state.Player.Character.Inventory.ActiveItem;
            string inventoryText = inventoryItem == null ? "" : 
                inventoryItem.Graphics.Hotspot ?? inventoryItem.CursorGraphics.Hotspot ?? "Item";
            
			_label.Text = getSentence(obj.Hotspot, inventoryText);

		}

		private string getSentence(string hotspot, string inventoryItem)
		{
			string format;
			if (_verbFormats.TryGetValue(_getMode(), out format))
			{
				return string.Format(format, hotspot, inventoryItem);
			}
			return hotspot;
		}
	}
}
