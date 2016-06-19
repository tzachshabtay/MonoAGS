namespace AGS.API
{
    [RequiredComponent(typeof(IUIEvents))]
    [RequiredComponent(typeof(IAnimationContainer))]
    public interface ICheckboxComponent : IComponent
    {
        bool Checked { get; set; }
        IEvent<CheckboxEventArgs> OnCheckChanged { get; }

        IAnimation NotCheckedAnimation { get; set; }
        IAnimation HoverNotCheckedAnimation { get; set; }
        IAnimation CheckedAnimation { get; set; }
        IAnimation HoverCheckedAnimation { get; set; }
    }
}
