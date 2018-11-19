using System;
namespace AGS.Engine.Desktop
{
    public static class VeldridExtensions
    {
        public static Veldrid.WindowState Convert(this API.WindowState state, bool hasBorder)
        {
            switch (state)
            {
                case API.WindowState.FullScreen: return hasBorder ? Veldrid.WindowState.FullScreen : Veldrid.WindowState.BorderlessFullScreen;
                case API.WindowState.Maximized: return Veldrid.WindowState.Maximized;
                case API.WindowState.Minimized: return Veldrid.WindowState.Minimized;
                case API.WindowState.Normal: return Veldrid.WindowState.Normal;
                default: throw new NotSupportedException(state.ToString());
            }
        }

        public static API.Rectangle Convert(this Veldrid.Rectangle rect)
        {
            return new API.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
