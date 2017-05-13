using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Factory for creating shaders (<see cref="IShader"/>). 
    /// </summary>
    public interface IShaderFactory
    {
        /// <summary>
        /// Creates a shader from the source code of the vertex and fragment shaders, which
        /// should be passed as regular text.
        /// </summary>
        /// <returns>The shader.</returns>
        /// <param name="vertexSource">Vertex source (pass null for using the standard vertex shader).</param>
        /// <param name="fragmentSource">Fragment source (pass null for using the standard fragment shader).</param>
        IShader FromText(string vertexSource, string fragmentSource);

        /// <summary>
        /// Creates a shader asynchronously from the source code of the vertex and fragment shaders, 
        /// which should be passed as the paths for the shader files (either an embedded resource path
        /// or a file system path).
        /// </summary>
        /// <returns>The shader.</returns>
        /// <param name="vertexResource">Vertex resource path (pass null for using the standard vertex shader).</param>
        /// <param name="fragmentResource">Fragment resource path (pass null for using the standard fragment shader).</param>
        Task<IShader> FromResource(string vertexResource, string fragmentResource);
    }
}
