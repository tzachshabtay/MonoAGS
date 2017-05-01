namespace AGS.API
{
    /// <summary>
    /// A pre-set entity with all of the UI control components, plus a text and textbox component,
    /// for allowing editing/inserting text.
    /// </summary>
    public interface ITextBox : IUIControl, ITextComponent, ITextBoxComponent
    {
    }
}
