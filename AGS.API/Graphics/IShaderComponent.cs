namespace AGS.API
{
    /// <summary>
    /// This component provides a way to attach a shader to your entities
    /// </summary>
    /// <seealso cref="AGS.API.IComponent" />
    public interface IShaderComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the shader that is attached to the entity.
        /// </summary>
        /// <value>
        /// The shader.
        /// </value>
        IShader Shader { get; set; }
	}
}

