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
        private IEntity _entity;

        public AGSComponentHost(Resolver resolver)
        {
            _resolver = resolver;
        }

		public override void Init(IEntity entity)
		{
            base.Init(entity);
            _entity = entity;
		}

        public TComponent AddComponent<TComponent>() where TComponent : IComponent
        {
            if (_entity.HasComponent<TComponent>()) return _entity.GetComponent<TComponent>();
            TComponent component = (TComponent)_resolver.Container.Resolve(typeof(TComponent));
            _entity.AddComponent<TComponent>(component);
            return component;
        }
	}
}