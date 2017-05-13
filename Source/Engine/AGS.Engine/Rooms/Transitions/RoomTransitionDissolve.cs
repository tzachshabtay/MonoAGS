using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class RoomTransitionDissolve : IRoomTransition
	{
		//shader code inspired by: http://developer.playcanvas.com/en/tutorials/advanced/custom-shaders/
        const string FRAGMENT_SHADER = @"
#ifdef GL_ES
            precision mediump float;
#endif
            uniform sampler2D uTexture;
            uniform float time;
#ifdef GL_ES
            varying vec4 vColor;
            varying vec2 vTexCoord;
#else
            varying vec4 gl_Color;
#endif

            void main()
            {
#ifndef GL_ES
                vec2 vTexCoord = gl_TexCoord[0].xy;
                vec4 vColor = gl_Color;
#endif
                vec4 col = texture2D(uTexture, vTexCoord);
                float height = col.r;
                if (height < time) 
                {
                    discard;
                }
                /*else if (height < time +0.04) add this if we want a 'burn' color
                {
                    gl_FragColor = vec4(0, 0.2, 1, 1.0);
                }*/
                else gl_FragColor = col * vColor;
                //gl_FragColor = vec4(1.0,0.0,0.0,1.0);
            }
";

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
            var shader = _game.Factory.Shaders.FromText(null, FRAGMENT_SHADER).Compile();
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

