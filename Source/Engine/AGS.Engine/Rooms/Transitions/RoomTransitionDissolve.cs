using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class RoomTransitionDissolve : IRoomTransition
	{
		//shader code inspired by: http://developer.playcanvas.com/en/tutorials/advanced/custom-shaders/
        const string FRAGMENT_SHADER = @"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
layout(set = 2, binding = 1) uniform TimeBuffer
{
    float time;
};

void main()
{
    vec4 col = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
    float height = col.r;
    if (height < time)
    {
        discard;
    }
    /*else if (height < time +0.04) add this if we want a 'burn' color
    {
        fsout_color = vec4(0, 0.2, 1, 1.0);
    }*/
    else fsout_color = col * fsin_color;
}"; 

        private readonly float _timeInSeconds;
		private readonly Func<float, float> _easing;
		private readonly QuadVectors _screenVectors;
        private readonly IGame _game;

		private float _time;
		private Tween _tween;
		private Action _visitTween;

        public RoomTransitionDissolve(IGLUtils glUtils, float timeInSeconds = 1f, Func<float, float> easing = null, IGame game = null)
		{
			_timeInSeconds = timeInSeconds;
			_easing = easing ?? Ease.Linear;
            _game = game ?? AGSGame.Game;
            _screenVectors = new QuadVectors (_game, glUtils);
		}

		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(List<IObject> displayList)
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
            var shader = _game.Factory.Shaders.FromText(null, FRAGMENT_SHADER).Compile(new ShaderVarsBuffer(ShaderMode.FragmentShader, "TimeBuffer"));
			if (shader == null)
			{
				return false;
			}
			shader.Bind();
			if (!shader.SetVariable("TimeBuffer", _time))
			{
				shader.Unbind();
				return false;
			}
			_screenVectors.Render(from.Texture);

			if (oldShader != null) oldShader.Bind();
			else shader.Unbind();

			return true;
		}

		public bool RenderAfterEnteringRoom(List<IObject> displayList)
		{
			return false;
		}

		#endregion
	}
}