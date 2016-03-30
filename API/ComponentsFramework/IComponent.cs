using System;

namespace AGS.API
{
	public interface IComponent : IDisposable
	{
		string Name { get; }
		bool AllowMultiple { get; }

		void Init(IEntity entity);
	}
}

