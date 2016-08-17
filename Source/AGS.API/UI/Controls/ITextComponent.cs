namespace AGS.API
{
    [RequiredComponent(typeof(IImageComponent))]
    public interface ITextComponent : IComponent
    {
        ITextConfig TextConfig { get; set; }
        string Text { get; set; }
        SizeF LabelRenderSize { get; set; }

        float TextHeight { get; }
        float TextWidth { get; }
    }
}
