namespace AGS.API
{
    [RequiredComponent(typeof(IUIEvents))]
    [RequiredComponent(typeof(ITransformComponent))]
    public interface IDraggableComponent : IComponent
    {
        bool IsDragEnabled { get; set; }
        bool IsCurrentlyDragged { get; }

        float? DragMinX { get; set; }
        float? DragMaxX { get; set; }
        float? DragMinY { get; set; }
        float? DragMaxY { get; set; }
    }
}
