namespace AGS.API
{
    public interface IComboBox<TControl> : IComboBoxComponent, IUIControl<TControl> where TControl : IUIControl<TControl>
    { }

    public interface IComboBox : IComboBox<IComboBox>
    { }
}
