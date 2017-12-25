using System;
using System.Diagnostics;
using SkiaSharp;
using OpenTK.Graphics.OpenGL;
using AGS.Engine.Desktop;

namespace AGS.Engine
{
    public class SkiaSurface
    {
        private GRContext _context;
        private int _texture;

        public static int Texture;

        public IntPtr Load()
        {
            var lib = MacDynamicLibraries.dlopen("/System/Library/Frameworks/OpenGL.framework/Versions/A/Libraries/libGL.dylib", 1);

            var glInterface = GRGlInterface.AssembleGlInterface((context, name) => {
                return MacDynamicLibraries.dlsym(lib, name);
            });

            bool b = glInterface.Validate();
            Debug.Assert(b);

            _context = GRContext.Create(GRBackend.OpenGL, glInterface);
            Debug.Assert(_context.Handle != IntPtr.Zero);

            _texture = GL.GenTexture();

            AGSGame.DoSomething = Draw;
            return glInterface.Handle;
        }

        public unsafe void Draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            fixed (int* p = &_texture)
            {
                var desc = new GRBackendTextureDesc
                {
                    Width = 500,
                    Height = 300,
                    Config = GRPixelConfig.Bgra8888,
                    Origin = GRSurfaceOrigin.TopLeft,
                    SampleCount = 0,
                    Flags = GRBackendTextureDescFlags.RenderTarget,
                    TextureHandle = new IntPtr(_texture)
                };
                using (var surface = SKSurface.CreateAsRenderTarget(_context, desc))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear(SKColors.Green);

                    using (var paint = new SKPaint())
                    {
                        paint.IsAntialias = true;
                        paint.Color = new SKColor(0x2c, 0x3e, 0x50);
                        paint.StrokeCap = SKStrokeCap.Round;

                        canvas.DrawOval(50f, 50f, 50f, 50f, paint);
                    }
                }
            }
        }
    }
}
