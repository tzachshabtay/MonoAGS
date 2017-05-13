using System.IO;
using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
    public class AGSShaderFactory : IShaderFactory
    {
        private readonly IResourceLoader _resources;
        private readonly Resolver _resolver;
        private readonly IGraphicsBackend _graphics;

        public AGSShaderFactory(IResourceLoader resources, IGraphicsBackend graphics, Resolver resolver)
        {
            _resources = resources;
            _resolver = resolver;
            _graphics = graphics;
        }

        public IShader FromText(string vertexSource, string fragmentSource)
        {
            vertexSource = vertexSource ?? _graphics.GetStandardVertexShader();
            fragmentSource = fragmentSource ?? _graphics.GetStandardFragmentShader();
            NamedParameter vertexParam = new NamedParameter(nameof(vertexSource), vertexSource);
            NamedParameter fragmentParam = new NamedParameter(nameof(fragmentSource), fragmentSource);
            return _resolver.Container.Resolve<IShader>(vertexParam, fragmentParam);
        }

        public async Task<IShader> FromResource(string vertexResource, string fragmentResource)
        {
            string vertexSource = await getSource(vertexResource);
            string fragmentSource = await getSource(fragmentResource);
            return FromText(vertexSource, fragmentSource);
        }

        private async Task<string> getSource(string path)
        {
            if (path == null) return null;

            var resource = _resources.LoadResource(path);
            using (StreamReader rdr = new StreamReader(resource.Stream))
            {
                return await rdr.ReadToEndAsync();
            }
        }
    }
}
