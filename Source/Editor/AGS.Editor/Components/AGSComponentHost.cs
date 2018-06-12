using System;
using AGS.API;
using AGS.Engine;
using Autofac;

namespace AGS.Editor
{
    /// <summary>
    /// This class allows adding components to an entity with an alternative resolver.
    /// It is used by the editor, to use the editor's resolver instead of the game's resolver,
    /// thus injecting alternative behaviors.
    /// </summary>
    public class AGSComponentHost : AGSComponent
    {
        private readonly Resolver _resolver;

        public AGSComponentHost(Resolver resolver)
        {
            _resolver = resolver;
        }

        public TComponent AddComponent<TComponent>() where TComponent : IComponent
        {
            if (Entity.HasComponent<TComponent>()) return Entity.GetComponent<TComponent>();
            TComponent component = (TComponent)_resolver.Container.Resolve(typeof(TComponent));
            Entity.AddComponent<TComponent>(component);
            return component;
        }
	}
}