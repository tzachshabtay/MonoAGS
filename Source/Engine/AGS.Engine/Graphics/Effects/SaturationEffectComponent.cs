using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IShaderComponent))]
    public class SaturationEffectComponent : AGSComponent, ISaturationEffectComponent
    {
        public const string SATURATION_FRAGMENT_SHADER =
            @"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
uniform float adjustment;
void main()
{
    vec4 col = texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords) * fsin_color;
    vec3 rgb = col.xyz;

    //code from: https://github.com/AnalyticalGraphicsInc/cesium/blob/master/Source/Shaders/Builtin/Functions/saturation.glsl
    const vec3 luminance = vec3(0.2125, 0.7154, 0.0721);
    vec3 intensity = vec3(dot(rgb, luminance));
    col.xyz = mix(intensity, rgb, adjustment);
    fsout_color =  col * fsin_color;
}";

        private IShader _shader;
        private IShaderComponent _target;
        private readonly IShaderFactory _factory;
        private readonly IGameEvents _events;

        public SaturationEffectComponent(IShaderFactory factory, IGameEvents events)
        {
            _factory = factory;
            _events = events;
        }

        [NumberEditorSlider(sliderMin: -2f, sliderMax: 2f, step: 0.1f)]
        [Property(Category = "Image")]
        public float Saturation { get; set; } = 1f;

		public override void Init()
		{
            base.Init();
            _shader = _factory.FromText(null, SATURATION_FRAGMENT_SHADER);
            if (Entity != null)
            {
                Entity.Bind<IShaderComponent>(c => { _target = c; c.Shader = _shader; }, _ => _shader = null);
            }
            else AGSGame.Shader = _shader;
            _events.OnBeforeRender.Subscribe(onBeforeRender);
		}

        private IShader getActiveShader()
        {
            return _target?.Shader ?? AGSGame.Shader;
        }

		private void onBeforeRender()
        {
            var shader = _shader;
            if (shader != null) shader = shader.Compile();
            if (shader == null || shader != getActiveShader())
            {
                _shader = null;
                AGSGame.Game.Events.OnBeforeRender.Unsubscribe(onBeforeRender);
                return;
            }
            shader.Bind();
            shader.SetVariable("adjustment", Saturation);
            shader.Unbind();
        }
    }
}
