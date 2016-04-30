using System;

namespace AGS.API
{
	public interface IShaderComponent : IComponent
	{
		IShader Shader { get; set; }
	}
}

