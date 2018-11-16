using AGS.API;

namespace AGS.Engine
{
	public class AGSShaderComponent : AGSComponent, IShaderComponent
	{
		public IShader Shader { get; set; }
	}
}