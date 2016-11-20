using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
	public class GLShader : IShader, IDisposable
	{
		private int _program;
		private string _vertexSource, _fragmentSource;
		private Dictionary<string, int> _variables;
		private Dictionary<int, int> _textures;
		private bool _isCompiled;
		private bool _hadCompilationErrors;
		private static int _maxTextureUnits = -1;
        private readonly IGraphicsBackend _graphics;

        private GLShader(string vertexSource, string fragmentSource, IGraphicsBackend graphics)
		{
            _graphics = graphics;
			_vertexSource = vertexSource;
			_fragmentSource = fragmentSource;
			_variables = new Dictionary<string, int> ();
			_textures = new Dictionary<int, int> ();
		}

        public static GLShader FromText(string vertexSource, string fragmentSource, IGraphicsBackend graphics = null)
		{
            graphics = graphics ?? Hooks.GraphicsBackend;
            return new GLShader (vertexSource, fragmentSource, graphics);
		}

        public static async Task<GLShader> FromResource(string vertexResource, string fragmentResource, IGraphicsBackend graphics = null)
		{
            graphics = graphics ?? Hooks.GraphicsBackend;
			ResourceLoader loader = new ResourceLoader ();
			string vertexSource = await getSource(vertexResource, loader);
			string fragmentSource = await getSource(fragmentResource, loader);
            return FromText(vertexSource, fragmentSource, graphics);
		}

		public IShader Compile()
		{
			if (_hadCompilationErrors) return null;
			if (_isCompiled) return this;
            if (!_graphics.AreShadersSupported())
			{
				Debug.WriteLine("Shaders are not supported on this system.");
				_hadCompilationErrors = true;
				return null;
			}

			_program = _graphics.CreateProgram();
            if (!compileShader(_fragmentSource, ShaderMode.FragmentShader) ||
			    !compileShader(_vertexSource, ShaderMode.VertexShader))
			{
				_hadCompilationErrors = true;
				return null;
			}

			if (!linkProgram())
			{
				_hadCompilationErrors = true;
				return null;
			}

			_isCompiled = true;
            return this;
		}
			
		public void Bind()
		{
			_graphics.UseProgram(_program);
			bindTextures();
            _graphics.SetShaderAppVars(_program);
        }

		public void Unbind()
		{
			_graphics.UseProgram(0);
		}

		public bool SetVariable(string name, float x)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;
			_graphics.Uniform1(location, x);
			return true;
		}

		public bool SetVariable(string name, Vector2 v)
		{
			return SetVariable(name, v.X, v.Y);
		}

		public bool SetVariable(string name, float x, float y)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;
			_graphics.Uniform2(location, x, y);
			return true;
		}

		public bool SetVariable(string name, Vector3 v)
		{
			return SetVariable(name, v.X, v.Y, v.Z);
		}

		public bool SetVariable(string name, float x, float y, float z)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;
			_graphics.Uniform3(location, x, y, z);
			return true;
		}

		public bool SetVariable(string name, Vector4 v)
		{
			return SetVariable(name, v.X, v.Y, v.Z, v.W);
		}

		public bool SetVariable(string name, Color c)
		{
			return SetVariable(name, c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
		}

		public bool SetVariable(string name, float x, float y, float z, float w)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;
			_graphics.Uniform4(location, x, y, z, w);
			return true;
		}

		/*public bool SetVariable(string name, Matrix4 matrix)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;
			_graphics.UniformMatrix4(location, ref matrix);
			return true;
		}*/

		public bool SetTextureVariable(string name, int texture)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;

			_textures[location] = texture;
			if (_textures.Count > getMaxTextureUnits())
			{
				Debug.WriteLine("Failed to add texture {0} to shader. Reached max texture units {1}.",
					name, getMaxTextureUnits());
				_textures.Remove(location);
				return false;
			}
			return true;
		}

		#region IDisposable implementation

		public void Dispose()
		{
			int program = _program;
			if (program == 0) return;
			_graphics.DeleteProgram(program);
		}

		#endregion

		private int getVariableLocation(string name)
		{
			if (_program == 0) return -1;
			return _variables.GetOrAdd(name, () =>
			{
				int location = _graphics.GetUniformLocation(_program, name);
				Debug.WriteLineIf(location == -1, string.Format("Variable name {0} not found in shader program.", name));
				return location;
			});
		}

		private static async Task<string> getSource(string path, ResourceLoader loader)
		{
			if (path == null) return null;

			var resource = loader.LoadResource(path);
			using (StreamReader rdr = new StreamReader (resource.Stream))
			{
				return await rdr.ReadToEndAsync();
			}
		}

		private bool compileShader(string source, ShaderMode shaderType)
		{
			if (string.IsNullOrEmpty(source)) return true;

			int shader = _graphics.CreateShader(shaderType);
			_graphics.ShaderSource(shader, source);
			_graphics.CompileShader(shader);
			string info = _graphics.GetShaderInfoLog(shader);
            int errorCode = _graphics.GetShaderCompilationErrorCode(shader);

			if (errorCode != 1)
			{
				Debug.WriteLine(string.Format("Failed to compile {0}.{4}Error code: {1}.{4}Error message(s): {2}.{4}Shader Source: {3}{4}",
					shaderType, errorCode, info ?? "null", source, Environment.NewLine));
				_graphics.DeleteShader(shader);
				_program = 0;
				return false;
			}

			_graphics.AttachShader(_program, shader);
			_graphics.DeleteShader(shader);
			return true;
		}

		private bool linkProgram()
		{
			_graphics.LinkProgram(_program);
			string info = _graphics.GetProgramInfoLog(_program);
            int errorCode = _graphics.GetProgramLinkErrorCode(_program);
			if (errorCode != 1)
			{
				Debug.WriteLine(string.Format("Failed to link shader program. Error code: {0}.{2}Error message(s): {1}{2}",
					errorCode, info ?? "null", Environment.NewLine));
				_graphics.DeleteProgram(_program);
				_program = 0;
				return false;
			}
			return true;
		}

		private void bindTextures()
		{
			int i = 0;
			foreach (var pair in _textures)
			{
				_graphics.Uniform1(pair.Key, i);
				_graphics.ActiveTexture(i);
                _graphics.BindTexture2D(pair.Value);
				i++;
			}
			_graphics.ActiveTexture(0);
		}

		private int getMaxTextureUnits()
		{
			if (_maxTextureUnits != -1) return _maxTextureUnits;
            _maxTextureUnits = _graphics.GetMaxTextureUnits();
			return _maxTextureUnits;
		}
	}
}

