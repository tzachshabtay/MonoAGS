using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class ShakeEffect
	{
		const string VERTEX_SHADER_SHAKE =
            @"
#version 450
layout(set = 0, binding = 0) uniform MvpBuffer
{
    mat4 Mvp;
};
layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in vec4 Color;
layout(location = 0) out vec2 fsin_texCoords;
layout(location = 1) out vec4 fsin_color;
layout(set = 2, binding = 1) uniform ShakeBuffer
{
    float time;
    float strength;
};

void main()
{
    vec4 pos = vec4(Position, 1., 1.);
    gl_Position = Mvp * pos;
    fsin_texCoords = TexCoords;
    fsin_color = Color;
    gl_Position.x += cos(time * 10.0) * strength;        
    gl_Position.y += cos(time * 15.0) * strength;
}
";

        private readonly IShaderComponent _target;
		private readonly float _decay;
		private float _strength;

		private IShader _shakeShader, _previousShader;
		private TaskCompletionSource<object> _taskCompletionSource;

        public ShakeEffect(float strength = 0.05f, float decay = 0.99f, IShaderComponent target = null)
		{
			_target = target;
			_strength = strength;
			_decay = decay;
		}

		public void Start()
		{
			_previousShader = getActiveShader();
			_taskCompletionSource = new TaskCompletionSource<object> (null);
            _shakeShader = AGSGame.Game.Factory.Shaders.FromText(VERTEX_SHADER_SHAKE, null);
			AGSGame.Game.Events.OnBeforeRender.Subscribe(onBeforeRender);
			setActiveShader(_shakeShader);
		}

		public void Stop()
		{
			_shakeShader = null;
			setActiveShader(_previousShader);
            _taskCompletionSource?.TrySetResult(null);
		}

		public async Task RunAsync(TimeSpan time)
		{
			Start();
			var tcs = _taskCompletionSource;
			await Task.WhenAny(tcs.Task, Task.Delay(time));
			Stop();
		}

		private void onBeforeRender()
		{
			var shader = _shakeShader;
			if (shader != null) shader = shader.Compile(new ShaderVarsBuffer(ShaderMode.VertexShader, "ShakeBuffer", 2));

            if (shader == null || shader != getActiveShader())
			{
				_shakeShader = null;
				AGSGame.Game.Events.OnBeforeRender.Unsubscribe(onBeforeRender);
				return;
			}
			shader.Bind();
            var currentStrength = _strength + MathUtils.Random().Next(1000) / 1000000f; //Adding a little jerkiness
            shader.SetVariable("ShakeBuffer", Repeat.Do("ShakeEffect"), currentStrength);
            shader.Unbind();
			_strength = _strength * _decay;
		}

		private IShader getActiveShader()
		{
			return _target?.Shader ?? AGSGame.Shader;
		}

		private void setActiveShader(IShader shader)
		{
			if (_target != null)
			{
				_target.Shader = shader;
				return;
			}
			AGSGame.Shader = shader;
		}
	}
}