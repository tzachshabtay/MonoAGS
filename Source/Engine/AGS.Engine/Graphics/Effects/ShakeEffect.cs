using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class ShakeEffect
	{
		const string VERTEX_SHADER_SHAKE = 
			@"
#ifdef GL_ES
    uniform mat4    uMvp;
    attribute vec2 aPosition;

    attribute vec2 aTexCoord;
    varying vec2 vTexCoord;

    attribute vec4 aColor;
    varying vec4 vColor;
#else
    varying vec4 gl_FrontColor;
#endif

    uniform float time;
    uniform float strength;
    void main() 
    {
#ifdef GL_ES
       vec4 position = vec4(aPosition.xy, 1., 1.);
       gl_Position = uMvp * position;
       vTexCoord = aTexCoord;
       vColor = aColor; 
#else
       gl_FrontColor = gl_Color;
       gl_TexCoord[0] = gl_MultiTexCoord0;
       gl_Position = ftransform();             
#endif
       gl_Position.x += cos(time * 10.0) * strength;        
       gl_Position.y += cos(time * 15.0) * strength;
    }
";

		private readonly IObject _target;
		private readonly float _decay;
		private float _strength;

		private IShader _shakeShader, _previousShader;
		private TaskCompletionSource<object> _taskCompletionSource;

        public ShakeEffect(float strength = 0.05f, float decay = 0.99f, IObject target = null)
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

		private void onBeforeRender()
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
            shader.SetVariable("time", (float)Repeat.Do("ShakeEffect"));
			var currentStrength = _strength + MathUtils.Random().Next(1000) / 1000000f; //Adding a little jerkiness
			shader.SetVariable("strength", currentStrength);
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

