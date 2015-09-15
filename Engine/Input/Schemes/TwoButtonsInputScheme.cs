using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class TwoButtonsInputScheme
	{
		private IGameState state;
		private IInputEvents input;
		GLText label;

		public TwoButtonsInputScheme (IGameState state, IInputEvents input, GLText label)
		{
			this.state = state;
			this.input = input;
			this.label = label;
		}

		public void Start()
		{
			input.MouseDown.SubscribeToAsync(onMouseDown);
			input.MouseMove.Subscribe (onMouseMove);
		}


		private async Task onMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!state.Player.Character.Enabled)
				return;
			//label.Text = string.Format ("{0},{1}", e.X, e.Y);

			if (e.Button == MouseButton.Left)
			{
				if (state.Player.Character.Inventory == null || 
					state.Player.Character.Inventory.ActiveItem == null)
				{
					AGSLocation location = new AGSLocation(e.X, e.Y, state.Player.Character.Z);
					await state.Player.Character.WalkAsync(location).ConfigureAwait(true);
					//label.Text = string.Format ("{0},{1}", state.Player.Character.X, 
					//	state.Player.Character.Y);
				}
				else
				{

				}
			}
			else if (e.Button == MouseButton.Right)
			{
				IInventory inventory = state.Player.Character.Inventory;
				if (inventory == null) return;
				if (inventory.ActiveItem == null)
				{
					IObject hotspot = state.Player.Character.Room.GetHotspotAt(e.X, e.Y);
					if (hotspot == null) return;
				}
				else
				{
					inventory.ActiveItem = null;
				}
			}
		}

		private void onMouseMove(object sender, MousePositionEventArgs e)
		{
			if (label == null) return;
			/*float angle = getAngle (state.Player.Character.X, state.Player.Character.Y,
				e.X, e.Y);
			label.Text = "Angle: " + ((int)angle).ToString();
			label.Visible = true;*/


			IObject obj = state.Player.Character.Room.GetHotspotAt (e.X, e.Y);
			if (obj == null || obj.Hotspot == null) 
			{
				//label.Visible = false;
				return;
			}
			label.Text = obj.Hotspot;
			//label.Visible = true;

			/*ISquare box = state.Player.Character.BoundingBox;
			if (box == null)
				return;
			label.Text = string.Format("{0},{1}: {2}", (int)e.X, (int)e.Y, box.ToString());
			label.Visible = true;*/

			//label.Visible = true;
			//label.Text = string.Format ("{0},{1}", e.X, e.Y);
		}

		/*private float getAngle(float x1, float y1, float x2, float y2)
		{
			float deltaX = x2 - x1;
			if (deltaX == 0f)
				deltaX = 0.001f;
			float deltaY = y2 - y1;
			float angle = ((float)Math.Atan2 (deltaY, deltaX)) * 180f / (float)Math.PI;
			return angle;
		}*/
	}
}

