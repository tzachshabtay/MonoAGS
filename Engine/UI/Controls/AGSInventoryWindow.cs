using System;
using AGS.API;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using Autofac;

namespace AGS.Engine
{
	public class AGSInventoryWindow : IInventoryWindow
	{
		private readonly IPanel _obj;
		private readonly IGameState _state;
		private readonly VisibleProperty _visible;
		private readonly EnabledProperty _enabled;

		private IList<IObject> _inventoryItems;
		private volatile bool _refreshNeeded;
		private SizeF _itemSize;
		private ICharacter _character;
		private int _topItem;
		private readonly IHasRoom _roomBehavior;

		public AGSInventoryWindow(IPanel panel, IGameEvents gameEvents, IGameState state, Resolver resolver)
		{
			_obj = panel;
			_state = state;
			_visible = new VisibleProperty (this);
			_enabled = new EnabledProperty (this);
			_inventoryItems = new List<IObject> (20);
			TreeNode = new AGSTreeNode<IObject> (this);

			TypedParameter panelParam = new TypedParameter (typeof(IObject), this);
			_roomBehavior = resolver.Container.Resolve<IHasRoom>(panelParam);

			gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

		public string ID { get { return _obj.ID; } }

		#region IInventoryWindow implementation

		public void ScrollUp()
		{
			TopItem = Math.Max(0, TopItem - ItemsPerRow);
		}

		public void ScrollDown()
		{
			TopItem = Math.Min(CharacterToUse.Inventory.Items.Count - 1, TopItem + ItemsPerRow); 
		}

		public SizeF ItemSize
		{
			get { return _itemSize; }
			set
			{
				_itemSize = value;
				_refreshNeeded = true;
			}
		}

		public ICharacter CharacterToUse 
		{
			get { return _character; }
			set
			{
				_character = value;
				_refreshNeeded = true;
			}
		}

		public int TopItem 
		{
			get { return _topItem; }
			set
			{
				_topItem = value;
				_refreshNeeded = true;
			}
		}

		public int ItemsPerRow { get { return (int)(Width / ItemSize.Width); } }

		public int RowCount { get { return (int)(Height / ItemSize.Height); } }

		#endregion

		#region IUIControl implementation

		public void ApplySkin(ILabel skin)
		{
			throw new NotImplementedException();
		}

		public IUIEvents Events { get { return _obj.Events; } }
		public bool IsMouseIn { get { return _obj.IsMouseIn; } }

		#endregion

		#region IObject implementation

		public ICustomProperties Properties { get { return _obj.Properties; } }

		public void StartAnimation(IAnimation animation)
		{
			_obj.StartAnimation(animation);
		}

		public AnimationCompletedEventArgs Animate(IAnimation animation)
		{
			return _obj.Animate(animation);
		}

		public async Task<AnimationCompletedEventArgs> AnimateAsync(IAnimation animation)
		{
			return await _obj.AnimateAsync(animation);
		}

		public void ChangeRoom(IRoom room, float? x = null, float? y = null)
		{
			_roomBehavior.ChangeRoom(room, x, y);
		}

		public bool CollidesWith(float x, float y)
		{
			return _obj.CollidesWith(x, y);
		}

		public IRoom Room { get { return _roomBehavior.Room; } }

		public IRoom PreviousRoom { get { return _roomBehavior.PreviousRoom; } }

		public IAnimation Animation { get { return _obj.Animation; } }

		public IInteractions Interactions { get { return _obj.Interactions; } }

		public ISquare BoundingBox { get { return _obj.BoundingBox; } set { _obj.BoundingBox = value; } }

		public void PixelPerfect(bool pixelPerfect)
		{
			_obj.PixelPerfect(pixelPerfect);
		}

		public IArea PixelPerfectHitTestArea
		{
			get
			{
				return _obj.PixelPerfectHitTestArea;
			}
		}

		public IRenderLayer RenderLayer { get { return _obj.RenderLayer; } set { _obj.RenderLayer = value; } }

		public ITreeNode<IObject> TreeNode { get; private set; }

		public bool Visible { get { return _visible.Value; } set { _visible.Value = value; } }

		public bool Enabled { get { return _enabled.Value; } set { _enabled.Value = value; } }

		public bool UnderlyingVisible { get { return _visible.UnderlyingValue; } }

		public bool UnderlyingEnabled { get { return _enabled.UnderlyingValue; } }

		public string Hotspot { get { return _obj.Hotspot; } set { _obj.Hotspot = value; } }

		public bool IgnoreViewport { get { return _obj.IgnoreViewport; } set { _obj.IgnoreViewport = value; } }
		public bool IgnoreScalingArea { get { return _obj.IgnoreScalingArea; } set { _obj.IgnoreScalingArea = value; } }

		public IPoint WalkPoint { get { return _obj.WalkPoint; } set { _obj.WalkPoint = value; } }
		public IPoint CenterPoint { get { return _obj.CenterPoint; } }

		public bool DebugDrawAnchor { get { return _obj.DebugDrawAnchor; } set { _obj.DebugDrawAnchor = value; } }

		public IBorderStyle Border { get { return _obj.Border; } set { _obj.Border = value; } }

		#endregion

		#region ISprite implementation

		public void ResetScale()
		{
			_obj.ResetScale();
		}

		public void ScaleBy(float scaleX, float scaleY)
		{
			_obj.ScaleBy(scaleX, scaleY);
		}

		public void ScaleTo(float width, float height)
		{
			_obj.ScaleTo(width, height);
		}

		public void FlipHorizontally()
		{
			_obj.FlipHorizontally();
		}

		public void FlipVertically()
		{
			_obj.FlipVertically();
		}

		public ISprite Clone()
		{
			return _obj.Clone();
		}

		public ILocation Location { get { return _obj.Location; } set { _obj.Location = value; } }

		public float X { get { return _obj.X; } set { _obj.X = value; } }

		public float Y { get { return _obj.Y; } set { _obj.Y = value; } }

		public float Z { get { return _obj.Z; } set { _obj.Z = value; } }

		public float Height { get { return _obj.Height; } }

		public float Width { get { return _obj.Width; } }

		public float ScaleX { get { return _obj.ScaleX; } }

		public float ScaleY { get { return _obj.ScaleY; } }

		public float Angle { get { return _obj.Angle; } set { _obj.Angle = value; } }

		public byte Opacity { get { return _obj.Opacity; } set { _obj.Opacity = value; } }

		public Color Tint { get { return _obj.Tint; } set { _obj.Tint = value; } }

		public IPoint Anchor { get { return _obj.Anchor; } set { _obj.Anchor = value; } }

		public IImage Image { get { return _obj.Image; } set { _obj.Image = value; } }

		public IImageRenderer CustomRenderer { get { return _obj.CustomRenderer; } set { _obj.CustomRenderer = value; } }

		#endregion

		public override string ToString()
		{
			return string.Format("Inventory window for: {0}", CharacterToUse == null ? "null" : CharacterToUse.ToString());
		}

		private void onRepeatedlyExecute(object sender, AGSEventArgs args)
		{
			if (_character == null) return;
			if (!isRefreshNeeded()) return;
			_refreshNeeded = false;

			foreach (var obj in _inventoryItems)
			{
				TreeNode.RemoveChild(obj);
				_state.UI.Remove(obj);
			}
			List<IObject> items = new List<IObject> (_character.Inventory.Items.Count);
			foreach (var item in _character.Inventory.Items)
			{
				items.Add(item.Graphics);
			}
			_inventoryItems = items;

			int topItem = TopItem;
			int count = Math.Min(topItem + RowCount * ItemsPerRow, items.Count);
			float stepX = ItemSize.Width;
			float stepY = ItemSize.Height;
			float x = stepX/2f;
			float y = Height - stepY/2;
			for (int item = topItem; item < count; item++)
			{
				IObject obj = items[item];
				obj.X = x;
				obj.Y = y;

				TreeNode.AddChild(obj);
				_state.UI.Add(obj);

				x += stepX;
				if (x >= Width)
				{
					x = stepX/2f;
					y -= stepY;
				}
			}
		}

		private bool isRefreshNeeded()
		{
			if (_refreshNeeded) return true;
			if (_inventoryItems.Count != _character.Inventory.Items.Count) return true;
			for (int i = 0; i < _inventoryItems.Count; i++)
			{
				if (_inventoryItems[i] != _character.Inventory.Items[i].Graphics) return true;
			}
			return false;
		}
	}
}

