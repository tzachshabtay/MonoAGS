using System;
namespace AGS.API
{
    public interface IRuntimeSettings : IGameSettings
    {
        new string Title { get; set; }
        new Size WindowSize { get; set; }
        new bool PreserveAspectRatio { get; set; }
        new WindowState WindowState { get; set; }
        new WindowBorder WindowBorder { get; set; }
        new VsyncMode Vsync { get; set; }

        void ResetViewport();
    }
}
