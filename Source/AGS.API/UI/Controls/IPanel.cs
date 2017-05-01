namespace AGS.API
{
    /// <summary>
    /// A panel is a UI control which hosts other UI controls.
    /// Note: the panel is in fact completely unnecessary, as any other UI control can host UI controls as well,
    /// but it's here as it provides a clear intent on its use.
    /// </summary>
    public interface IPanel : IUIControl
	{}    
}

