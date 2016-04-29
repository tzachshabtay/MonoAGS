using System;
using AGS.Engine;

namespace DemoQuest
{
	public static class Shaders
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

		const string FRAGMENT_SHADER_GRAYSCALE = 
@"#version 120

uniform sampler2D texture;
varying vec4 gl_Color;

void main()
{
	vec4 col = texture2D(texture, gl_TexCoord[0].st) * gl_Color;
	float gray = dot(col.rgb, vec3(0.299, 0.587, 0.114));
	gl_FragColor = vec4(gray, gray, gray, col.a);
}";

		const string FRAGMENT_SHADER_SEPIA = 
@"#version 120

uniform sampler2D texture;
varying vec4 gl_Color;
const vec3 SEPIA = vec3(1.2, 1.0, 0.8); 

void main()
{
	vec4 col = texture2D(texture, gl_TexCoord[0].st) * gl_Color;
	float gray = dot(col.rgb, vec3(0.299, 0.587, 0.114));
	gl_FragColor = vec4(vec3(gray) * SEPIA, col.a);
}";

		const string FRAGMENT_SHADER_SOFT_SEPIA = 
			@"#version 120

uniform sampler2D texture;
varying vec4 gl_Color;
const vec3 SEPIA = vec3(1.2, 1.0, 0.8); 

void main()
{
	vec4 col = texture2D(texture, gl_TexCoord[0].st);
	float gray = dot(col.rgb, vec3(0.299, 0.587, 0.114));
	vec3 sepiaColor = vec3(gray) * SEPIA;
	col.rgb = mix(col.rgb, sepiaColor, 0.75);
	gl_FragColor = col * gl_Color;
}";

		public static void SetStandardShader()
		{
			AGSGame.Shader = GLShader.FromText(VERTEX_SHADER, FRAGMENT_SHADER_STANDARD);
		}

		public static void SetGrayscaleShader()
		{
			AGSGame.Shader =  GLShader.FromText(VERTEX_SHADER, FRAGMENT_SHADER_GRAYSCALE);
		}

		public static void SetSepiaShader()
		{
			AGSGame.Shader =  GLShader.FromText(VERTEX_SHADER, FRAGMENT_SHADER_SEPIA);
		}

		public static void SetSoftSepiaShader()
		{
			AGSGame.Shader =  GLShader.FromText(VERTEX_SHADER, FRAGMENT_SHADER_SOFT_SEPIA);
		}

		public static void TurnOffShader()
		{
			AGSGame.Shader = null;
		}
	}
}

