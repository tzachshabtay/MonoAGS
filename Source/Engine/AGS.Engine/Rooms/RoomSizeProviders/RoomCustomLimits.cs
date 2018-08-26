using AGS.API;

namespace AGS.Engine
{
    [ConcreteImplementation(DisplayName = "Custom Limits")]
    public class RoomCustomLimits : IRoomLimitsProvider
    {
        RectangleF _customLimits;

        public static readonly RectangleF MaxLimits = 
            new RectangleF(-float.MaxValue, -float.MaxValue, float.MaxValue, float.MaxValue);

        public RoomCustomLimits(RectangleF customLimits)
        {
            _customLimits = customLimits;
        }

        public RectangleF ProvideRoomLimits(IRoom room)
        {
            return _customLimits;
        }

        public override string ToString() => $"Custom: {_customLimits.ToInspectorString()}";
    }
}
