using System;
using AGS.API;

namespace AGS.Engine
{
    public class FrameEventArgs : EventArgs
    {
        public double Time { get; set; }
    }

    public interface IGameWindow : IDisposable
    {
        event Action Load;
        event Action<System.Drawing.Size> Resize;
        event EventHandler<FrameEventArgs> UpdateFrame;
        event EventHandler<FrameEventArgs> RenderFrame;

        double TargetUpdateFrequency { get; set; }
        string Title { get; set; }
        VsyncMode Vsync { get; set; }
        WindowState WindowState { get; set; }
        WindowBorder WindowBorder { get; set; }
        int Width { get; }
        int Height { get; }
        int ClientWidth { get; }
        int ClientHeight { get; }
        bool IsExiting { get; }
        void SetSize(Size size);

        void Run(double updateRate);
        void SwapBuffers();
        void Exit();
    }
}
