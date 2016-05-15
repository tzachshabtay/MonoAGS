using System;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK;
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

		private GLShader(string vertexSource, string fragmentSource)
		{
			_vertexSource = vertexSource;
			_fragmentSource = fragmentSource;
			_variables = new Dictionary<string, int> ();
			_textures = new Dictionary<int, int> ();
		}

		public static GLShader FromText(string vertexSource, string fragmentSource)
		{
			return new GLShader (vertexSource, fragmentSource);
		}

		public static async Task<GLShader> FromResource(string vertexResource, string fragmentResource)
		{
			ResourceLoader loader = new ResourceLoader ();
			string vertexSource = await getSource(vertexResource, loader);
			string fragmentSource = await getSource(fragmentResource, loader);
			return FromText(vertexSource, fragmentSource);
		}

		public static bool IsSupported
		{
			get
			{
				return (new Version(GL.GetString(StringName.Version).Substring(0, 3)) >= new Version(2, 0) ? true : false);
			}
		}

		public IShader Compile()
		{
			if (_hadCompilationErrors) return null;
			if (_isCompiled) return this;
			if (!IsSupported)
			{
				Debug.WriteLine("Shaders are not supported on this system.");
				_hadCompilationErrors = true;
				return null;
			}

			_program = GL.CreateProgram();
			if (!compileShader(_fragmentSource, ShaderType.FragmentShader) ||
			    !compileShader(_vertexSource, ShaderType.VertexShader))
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
			GL.UseProgram(_program);
			bindTextures();
		}

		public void Unbind()
		{
			GL.UseProgram(0);
		}

		public bool SetVariable(string name, float x)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;
			GL.Uniform1(location, x);
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
			GL.Uniform2(location, x, y);
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
			GL.Uniform3(location, x, y, z);
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
			GL.Uniform4(location, x, y, z, w);
			return true;
		}

		public bool SetVariable(string name, Matrix4 matrix)
		{
			int location = getVariableLocation(name);
			if (location == -1) return false;
			GL.UniformMatrix4(location, false, ref matrix);
			return true;
		}

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
			GL.DeleteProgram(program);
		}

		#endregion

		private int getVariableLocation(string name)
		{
			if (_program == 0) return -1;
			return _variables.GetOrAdd(name, () =>
			{
				int location = GL.GetUniformLocation(_program, name);
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

		private bool compileShader(string source, ShaderType shaderType)
		{
			if (string.IsNullOrEmpty(source)) return true;

			int shader = GL.CreateShader(shaderType);
			GL.ShaderSource(shader, source);
			GL.CompileShader(shader);
			string info = GL.GetShaderInfoLog(shader);
			int errorCode;
			GL.GetShader(shader, ShaderParameter.CompileStatus, out errorCode);

			if (errorCode != 1)
			{
				Debug.WriteLine(string.Format("Failed to compile {0}.{4}Error code: {1}.{4}Error message(s): {2}.{4}Shader Source: {3}{4}",
					shaderType, errorCode, info ?? "null", source, Environment.NewLine));
				GL.DeleteShader(shader);
				_program = 0;
				return false;
			}

			GL.AttachShader(_program, shader);
			GL.DeleteShader(shader);
			return true;
		}

		private bool linkProgram()
		{
			GL.LinkProgram(_program);
			string info = GL.GetProgramInfoLog(_program);
			int errorCode;
			GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out errorCode);
			if (errorCode != 1)
			{
				Debug.WriteLine(string.Format("Failed to link shader program. Error code: {0}.{2}Error message(s): {1}{2}",
					errorCode, info ?? "null", Environment.NewLine));
				GL.DeleteProgram(_program);
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
				GL.Uniform1(pair.Key, i);
				GL.ActiveTexture(TextureUnit.Texture0 + i);
				GL.BindTexture(TextureTarget.Texture2D, pair.Value);
				i++;
			}
			GL.ActiveTexture(TextureUnit.Texture0);
		}

		private int getMaxTextureUnits()
		{
			if (_maxTextureUnits != -1) return _maxTextureUnits;
			_maxTextureUnits = GL.GetInteger(GetPName.MaxTextureUnits);
			return _maxTextureUnits;
		}
	}
}

