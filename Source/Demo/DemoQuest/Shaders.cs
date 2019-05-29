using System;
using AGS.Engine;
using AGS.API;

namespace DemoQuest
{
    //Shaders source taken from: https://github.com/mattdesl/lwjgl-basics/wiki/ShaderLesson3 & https://www.youtube.com/watch?v=qNM0k522R7o
    public static class Shaders
    {
        private static IShaderFactory _shaders => AGSGame.Game.Factory.Shaders;

        const string FRAGMENT_SHADER_GRAYSCALE =
@"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
void main()
{
    vec4 col = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords) * fsin_color;
    float gray = dot(col.rgb, vec3(0.299, 0.587, 0.114));
    fsout_color =  vec4(gray, gray, gray, col.a);
}";

		const string FRAGMENT_SHADER_SEPIA =
@"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
const vec3 SEPIA = vec3(1.2, 1.0, 0.8);
void main()
{
    vec4 col = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords) * fsin_color;
    float gray = dot(col.rgb, vec3(0.299, 0.587, 0.114));
    fsout_color =  vec4(vec3(gray) * SEPIA, col.a);
}";

		const string FRAGMENT_SHADER_SOFT_SEPIA =
@"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
const vec3 SEPIA = vec3(1.2, 1.0, 0.8);
void main()
{
    vec4 col = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
    float gray = dot(col.rgb, vec3(0.299, 0.587, 0.114));
    vec3 sepiaColor = vec3(gray) * SEPIA;
    col.rgb = mix(col.rgb, sepiaColor, 0.75);
    fsout_color =  col * fsin_color;
}";

        const string FRAGMENT_SHADER_VIGNETTE =
@"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;

//The resolution needs to be set whenever the screen resizes
layout(set = 2, binding = 1) uniform ResolutionBuffer
{
    vec2 resolution;
};

//RADIUS of our vignette, where 0.5 results in a circle fitting the screen
const float RADIUS = 0.75;

//softness of our vignette, between 0.0 and 1.0
const float SOFTNESS = 0.45;

void main()
{
    vec4 col = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);

    //determine center position
    vec2 position = (gl_FragCoord.xy / resolution.xy) - vec2(0.5);

    //determine the vector length of the center position
    float len = length(position);

    //use smoothstep to create a smooth vignette
    float vignette = smoothstep(RADIUS, RADIUS-SOFTNESS, len);

    //apply the vignette with 50% opacity
    col.rgb = mix(col.rgb, col.rgb * vignette, 0.5);
    fsout_color =  col * fsin_color;
}";

        const string FRAGMENT_SHADER_BLUR =
            @"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;

void main()
{
    int i, j;
    vec4 sum = vec4(0);
    for (i = -2; i <= 2; i++)
        for (j = -2; j <= 2; j++)
        {
            vec2 offset = vec2(i, j) * 0.01;
            vec4 currentCol = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords + offset);
            sum += currentCol;
        }
    fsout_color =  (sum / vec4(25));
}";

		public static void SetStandardShader()
		{
			unbindVignetteShader();
            AGSGame.Shader = _shaders.FromText(null, null);
		}

		public static void SetGrayscaleShader()
		{
			unbindVignetteShader();
			AGSGame.Shader =  _shaders.FromText(null, FRAGMENT_SHADER_GRAYSCALE);
		}

		public static void SetSepiaShader()
		{
			unbindVignetteShader();
			AGSGame.Shader =  _shaders.FromText(null, FRAGMENT_SHADER_SEPIA);
		}

		public static void SetSoftSepiaShader()
		{
			unbindVignetteShader();
			AGSGame.Shader =  _shaders.FromText(null, FRAGMENT_SHADER_SOFT_SEPIA);
		}

		public static void SetBlurShader()
		{
			unbindVignetteShader();
			AGSGame.Game.State.Player.Shader = _shaders.FromText(null, FRAGMENT_SHADER_BLUR);
		}

		private static IShader _vignetteShader;
		public static void SetVignetteShader()
		{
			_vignetteShader = _shaders.FromText(null, FRAGMENT_SHADER_VIGNETTE);
			AGSGame.Game.Events.OnBeforeRender.Subscribe(firstSetupVignette);
			AGSGame.Shader = _vignetteShader;
		}

		private static void firstSetupVignette()
		{
			setVignetteResolution();
			AGSGame.Game.Events.OnBeforeRender.Unsubscribe(firstSetupVignette);
			AGSGame.Game.Events.OnScreenResize.Subscribe(onVignetteShaderResize);
		}

		private static void onVignetteShaderResize()
		{
			setVignetteResolution();
		}

		private static void setVignetteResolution()
		{
            var resolution = AGSGame.Game.Settings.WindowSize;
			_vignetteShader.Compile(new ShaderVarsBuffer(ShaderMode.FragmentShader, "ResolutionBuffer", 2));
			_vignetteShader.Bind();
			_vignetteShader.SetVariable("ResolutionBuffer", resolution.Width, resolution.Height);
            _vignetteShader.Unbind();
		}

		private static void unbindVignetteShader()
		{
			var shader = _vignetteShader;
			if (shader == null) return;
			AGSGame.Game.Events.OnScreenResize.Unsubscribe(onVignetteShaderResize);
			_vignetteShader = null;
		}

		public static async void SetShakeShader()
		{
			unbindVignetteShader();
			ShakeEffect effect = new ShakeEffect ();
			await effect.RunAsync(TimeSpan.FromSeconds(5));
		}

		public static void TurnOffShader()
		{
			unbindVignetteShader();
			AGSGame.Shader = null;
			AGSGame.Game.State.Player.Shader = null;
		}
	}
}