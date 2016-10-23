using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class ShakeEffect
	{
		const string VERTEX_SHADER_SHAKE = 
			@"#version 120

varying vec4 gl_FrontColor;
uniform float time;
uniform float strength;

void main(void)
{
	gl_FrontColor = gl_Color;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_Position = ftransform();
	gl_Position.x += cos(time * 10) * strength;        
	gl_Position.y += cos(time * 15) * strength;
}
";

		const string FRAGMENT_SHADER_STANDARD = 
			@"#version 120

uniform sampler2D texture;
varying vec4 gl_Color;

void main()
{
	vec2 pos = gl_TexCoord[0].xy;
	vec4 col = texture2D(texture, pos);
	gl_FragColor = col * gl_Color;
}";
		
		private readonly IObject _target;
		private readonly float _decay;
        private readonly IGraphicsBackend _graphics;
		private float _strength;

		private IShader _shakeShader, _previousShader;
		private TaskCompletionSource<object> _taskCompletionSource;

        public ShakeEffect(float strength = 0.05f, float decay = 0.99f, IObject target = null, IGraphicsBackend graphics = null)
		{
            _graphics = graphics ?? Hooks.GraphicsBackend;
			_target = target;
			_strength = strength;
			_decay = decay;
		}

		public void Start()
		{
			_previousShader = getActiveShader();
			_taskCompletionSource = new TaskCompletionSource<object> (null);
			_shakeShader = GLShader.FromText(VERTEX_SHADER_SHAKE, FRAGMENT_SHADER_STANDARD, _graphics);
			AGSGame.Game.Events.OnBeforeRender.Subscribe(onBeforeRender);
			setActiveShader(_shakeShader);
		}

		public void Stop()
		{
			_shakeShader = null;
			setActiveShader(_previousShader);
			var tcs = _taskCompletionSource;
			if (tcs != null) tcs.TrySetResult(null);
		}

		public void RunBlocking(TimeSpan time)
		{
			Task.Run(async () => await RunAsync(time)).Wait();
		}

		public async Task RunAsync(TimeSpan time)
		{
			Start();
			var tcs = _taskCompletionSource;
			await Task.WhenAny(tcs.Task, Task.Delay(time));
			Stop();
		}

		private void onBeforeRender(object sender, AGSEventArgs args)
		{
			var shader = _shakeShader;
			if (shader != null) shader = shader.Compile();
			if (shader == null || shader != getActiveShader())
			{
				_shakeShader = null;
				AGSGame.Game.Events.OnBeforeRender.Unsubscribe(onBeforeRender);
				return;
			}
			shader.Bind();
			shader.SetVariable("time", (float)args.TimesInvoked);
			var currentStrength = _strength + MathUtils.Random().Next(1000) / 1000000f; //Adding a little jerkiness
			shader.SetVariable("strength", currentStrength);
			_strength = _strength * _decay;
		}

		private IShader getActiveShader()
		{
			if (_target != null) return _target.Shader;
			return AGSGame.Shader;
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

