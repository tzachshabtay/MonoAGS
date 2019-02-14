using System;
using AGS.API;

using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSInventoryWindowComponent : AGSComponent, IInventoryWindowComponent
	{
		private IList<IObject> _inventoryItems;
		private volatile bool _refreshNeeded;
		private SizeF _itemSize, _paddingBetweenItems;
        private IInventory _inventory;
		private int _topItem;
		private IGameState _state;
		private IGameEvents _gameEvents;
		private IScale _scale;
		private IInObjectTreeComponent _tree;
        private float _paddingLeft, _paddingRight, _paddingTop, _paddingBottom;
        private float _lastWidth, _lastHeight;

		public AGSInventoryWindowComponent(IGameState state, IGameEvents gameEvents)
		{
			_state = state;
			_inventoryItems = new List<IObject> (20);
			_gameEvents = gameEvents;
			_gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

		public override void Init()
		{
			base.Init();
            Entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            Entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
		}

		public override void Dispose()
		{
			base.Dispose();
			_gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
		}

		#region IInventoryWindow implementation

		public void ScrollUp()
		{
			TopItem = Math.Max(0, TopItem - ItemsPerRow);
		}

		public void ScrollDown()
		{
			TopItem = Math.Min(Inventory.Items.Count - 1, TopItem + ItemsPerRow); 
		}

		public SizeF ItemSize
		{
            get => _itemSize;
            set
			{
				_itemSize = value;
				_refreshNeeded = true;
			}
		}

        public IInventory Inventory 
		{
            get => _inventory;
            set
			{
				_inventory = value;
				_refreshNeeded = true;
			}
		}

		public int TopItem 
		{
            get => _topItem;
            set
			{
				_topItem = value;
				_refreshNeeded = true;
			}
		}

        public int ItemsPerRow => (int)((_scale.Width - _paddingLeft - _paddingRight) / (_itemSize.Width + _paddingBetweenItems.Width));

        public int RowCount => (int)((_scale.Height - _paddingTop - _paddingBottom) / (_itemSize.Height + _paddingBetweenItems.Height));

        public float PaddingLeft
        {
            get => _paddingLeft;
            set 
            {
                _paddingLeft = value;
                _refreshNeeded = true;
            }
        }

        public float PaddingRight
        {
            get => _paddingRight;
            set
            {
                _paddingRight = value;
                _refreshNeeded = true;
            }
        }

        public float PaddingTop
        {
            get => _paddingTop;
            set
            {
                _paddingTop = value;
                _refreshNeeded = true;
            }
        }

        public float PaddingBottom
        {
            get => _paddingBottom;
            set
            {
                _paddingBottom = value;
                _refreshNeeded = true;
            }
        }

        public SizeF PaddingBetweenItems
        {
            get => _paddingBetweenItems;
            set
            {
                _paddingBetweenItems = value;
                _refreshNeeded = true;
            }
        }

        #endregion

        private void onRepeatedlyExecute()
		{
			if (Inventory == null) return;
			if (!isRefreshNeeded()) return;
            var tree = _tree;
            if (tree == null) return;
			_refreshNeeded = false;

			foreach (var obj in _inventoryItems)
			{
                obj.Visible = false;	
                tree.TreeNode.AddChild(obj);
            }
			List<IObject> items = new List<IObject> (Inventory.Items.Count);
			foreach (var item in Inventory.Items)
			{
				items.Add(item.Graphics);
			}
			_inventoryItems = items;

			int topItem = TopItem;
			int count = Math.Min(topItem + RowCount * ItemsPerRow, items.Count);
			float stepX = _itemSize.Width + _paddingBetweenItems.Width;
			float stepY = _itemSize.Height + _paddingBetweenItems.Height;
			float left = _itemSize.Width/2f + _paddingLeft;
            float right = _scale.Width - _paddingRight - _itemSize.Width / 2f;
            float x = left;
			float y = _scale.Height - _itemSize.Height / 2 - _paddingTop;
			for (int item = topItem; item < count; item++)
			{
				IObject obj = items[item];
				obj.X = x;
				obj.Y = y;

                _tree.TreeNode.AddChild(obj);
                if (!_state.UI.Contains(obj))
                {
                    _state.UI.Add(obj);
                }
                obj.Visible = true;

				x += stepX;
				if (x >= right)
				{
                    x = left;
					y -= stepY;
				}
			}
		}

		private bool isRefreshNeeded()
		{
			if (_refreshNeeded) return true;
            if (!MathUtils.FloatEquals(_lastWidth, _scale.Width) || !MathUtils.FloatEquals(_lastHeight, _scale.Height))
            {
                _lastWidth = _scale.Width;
                _lastHeight = _scale.Height;
                return true; 
            }
            if (_inventoryItems.Count != Inventory.Items.Count) return true;
			for (int i = 0; i < _inventoryItems.Count; i++)
			{
				var item = _inventoryItems[i];
				if (item != Inventory.Items[i].Graphics) return true;
				if (item.TreeNode.Parent == null) return true;
			}
			return false;
		}
	}
}