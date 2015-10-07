using System;
using AGS.API;
using AGS.Engine;
using System.Drawing;

namespace DemoGame
{
	public class OptionsPanel
	{
		private const string _sliderFolder = "../../Assets/Gui/Sliders/";
		private IPanel _panel;

		public OptionsPanel()
		{
		}

		public void Load(IGameFactory factory)
		{
			_panel = factory.GetPanel("../../Assets/Gui/DialogBox/options.bmp", 160f, 100f);
			_panel.Anchor = new AGSPoint (0.5f, 0.5f);
			_panel.Visible = false;

			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig { TransparentColorSamplePoint = new Point (0, 0) };

			AGSTextConfig textConfig = new AGSTextConfig (font: new Font (SystemFonts.DefaultFont.FontFamily, 10f), brush: Brushes.DarkOliveGreen,
				outlineBrush: Brushes.LightGreen, outlineWidth: 1f);

			ISlider volumeSlider = factory.GetSlider(_sliderFolder + "slider.bmp", _sliderFolder + "handle.bmp", 50f, 0f, 100f, 
				loadConfig: loadConfig);
			volumeSlider.X = 120f;
			volumeSlider.Y = 10f;
			volumeSlider.HandleGraphics.Anchor = new AGSPoint (0.5f, 0.5f);
			volumeSlider.TreeNode.SetParent(_panel.TreeNode);

			ILabel volumeLabel = factory.GetLabel("Volume", 50f, 30f, 120f, 85f, textConfig); 
			volumeLabel.Anchor = new AGSPoint (0.5f, 0f);
			volumeLabel.TreeNode.SetParent(_panel.TreeNode);

			ISlider speedSlider = factory.GetSlider(_sliderFolder + "slider.bmp", _sliderFolder + "handle.bmp", 50f, 0f, 100f, 
				loadConfig: loadConfig);
			speedSlider.X = 180f;
			speedSlider.Y = 10f;
			speedSlider.HandleGraphics.Anchor = new AGSPoint (0.5f, 0.5f);
			speedSlider.TreeNode.SetParent(_panel.TreeNode);

			ILabel speedLabel = factory.GetLabel("Speed", 50f, 30f, 180f, 85f, textConfig); 
			speedLabel.Anchor = new AGSPoint (0.5f, 0f);
			speedLabel.TreeNode.SetParent(_panel.TreeNode);
		}

		public void Show()
		{
			_panel.Visible = true;
		}
	}
}

