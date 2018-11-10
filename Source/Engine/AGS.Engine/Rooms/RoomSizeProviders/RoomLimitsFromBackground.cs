using AGS.API;

namespace AGS.Engine
{
    [ConcreteImplementation(DisplayName = "From Room Background")]
    public class RoomLimitsFromBackground : IRoomLimitsProvider
    {
        public RectangleF ProvideRoomLimits(IRoom room)
        {
            if (room.Background == null) return RoomCustomLimits.MaxLimits;
            ISprite sprite = room.Background.CurrentSprite;
            return new RectangleF(0f, 0f, sprite.Width, sprite.Height);
        }

        public override string ToString() => "From Room Background";
    }
}
