using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeStringNode : AGSComponent, ITreeStringNode
    {
        public AGSTreeStringNode()
        {
            TreeNode = new AGSTreeNode<ITreeStringNode>(this);
            IdleTextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, font: AGSGameSettings.DefaultSpeechFont);
            HoverTextConfig = AGSTextConfig.ChangeColor(IdleTextConfig, Colors.Yellow, Colors.Black, 1f);
        }

        public ITextConfig HoverTextConfig { get; set; }

        public ITextConfig IdleTextConfig { get; set; }

        public string Text { get; set; }

        public ITreeNode<ITreeStringNode> TreeNode { get; private set; }
    }
}
