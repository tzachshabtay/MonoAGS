using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class RoomTransitionDissolve : IRoomTransition
	{
		const string VERTEX_SHADER = 
			@"#version 120

varying vec4 gl_FrontColor;

void main(void)
{
	gl_FrontColor = gl_Color;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_Position = ftransform();
}
";

		//shader code inspired by: http://developer.playcanvas.com/en/tutorials/advanced/custom-shaders/
		const string FRAGMENT_SHADER = 
			@"#version 120

uniform float time;
uniform sampler2D texture;
varying vec4 gl_Color;

			void main()
{
	vec2 pos = gl_TexCoord[0].xy;
	vec4 col = texture2D(texture, pos);
	float height = col.r;
	if (height < time) 
	{
		discard;
	}
	/*else if (height < time +0.04) add this if we want a 'burn' color
	{
		gl_FragColor = vec4(0, 0.2, 1, 1.0);
	}*/
	else gl_FragColor = col * gl_Color;
}";
		private readonly float _timeInSeconds;
		private readonly Func<float, float> _easing;
		private readonly QuadVectors _screenVectors;

		private float _time;
		private Tween _tween;
		private Action _visitTween;

		public RoomTransitionDissolve(float timeInSeconds = 1f, Func<float, float> easing = null, IGame game = null)
		{
			_timeInSeconds = timeInSeconds;
			_easing = easing ?? Ease.Linear;
			game = game ?? AGSGame.Game;
			_screenVectors = new QuadVectors (game);
		}

		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			_tween = Tween.RunWithExternalVisit(0f, 1f, t => _time = t, _timeInSeconds, _easing, out _visitTween);
			return false;
		}

		public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		{
			if (_tween.Task.IsCompleted)
			{
				return false;
			}
			_visitTween();
			var oldShader = AGSGame.Shader;
			_screenVectors.Render(to.Texture);
			var shader = GLShader.FromText(VERTEX_SHADER, FRAGMENT_SHADER).Compile();
			if (shader == null)
			{
				return false;
			}
			shader.Bind();
			if (!shader.SetVariable("time", _time))
			{
				shader.Unbind();
				return false;
			}
			_screenVectors.Render(from.Texture);

			if (oldShader != null) oldShader.Bind();
			else shader.Unbind();

			return true;
		}

		public bool RenderAfterEnteringRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			return false;
		}

		#endregion
	}
}

