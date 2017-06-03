namespace AGS.API
{
    public interface ITreeStringNode : IInTree<ITreeStringNode>
    {
        string Text { get; set; }
        ITextConfig IdleTextConfig { get; set; }
        ITextConfig HoverTextConfig { get; set; }
    }
}
