using System;
using AGS.API;

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

        public static API.GraphicsBackend Convert(this Veldrid.GraphicsBackend backend)
        {
            switch (backend)
            {
                case Veldrid.GraphicsBackend.Direct3D11:
                    return API.GraphicsBackend.Direct3D11;
                case Veldrid.GraphicsBackend.Metal:
                    return API.GraphicsBackend.Metal;
                case Veldrid.GraphicsBackend.OpenGL:
                    return API.GraphicsBackend.OpenGL;
                case Veldrid.GraphicsBackend.OpenGLES:
                    return API.GraphicsBackend.OpenGLES;
                case Veldrid.GraphicsBackend.Vulkan:
                    return API.GraphicsBackend.Vulkan;
                default:
                    throw new NotSupportedException(backend.ToString());
            }
        }

        public static Veldrid.GraphicsBackend Convert(this API.GraphicsBackend backend)
        {
            switch (backend)
            {
                case API.GraphicsBackend.Direct3D11:
                    return Veldrid.GraphicsBackend.Direct3D11;
                case API.GraphicsBackend.Metal:
                    return Veldrid.GraphicsBackend.Metal;
                case API.GraphicsBackend.OpenGL:
                    return Veldrid.GraphicsBackend.OpenGL;
                case API.GraphicsBackend.OpenGLES:
                    return Veldrid.GraphicsBackend.OpenGLES;
                case API.GraphicsBackend.Vulkan:
                    return Veldrid.GraphicsBackend.Vulkan;
                default:
                    throw new NotSupportedException(backend.ToString());
            }
        }

        public static Veldrid.ShaderStages Convert(this ShaderMode mode)
        { 
            switch (mode)
            {
                case ShaderMode.FragmentShader:
                    return Veldrid.ShaderStages.Fragment;
                case ShaderMode.VertexShader:
                    return Veldrid.ShaderStages.Vertex;
                case ShaderMode.ComputeShader:
                    return Veldrid.ShaderStages.Compute;
                case ShaderMode.GeometryShader:
                case ShaderMode.GeometryShaderExt:
                    return Veldrid.ShaderStages.Geometry;
                case ShaderMode.TessControlShader:
                    return Veldrid.ShaderStages.TessellationControl;
                case ShaderMode.TessEvaluationShader:
                    return Veldrid.ShaderStages.TessellationEvaluation;
                default:
                    throw new NotSupportedException(mode.ToString());
            }
        }
    }
}
