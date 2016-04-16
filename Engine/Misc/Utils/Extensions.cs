using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
	public static class Extensions
	{
		public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, Func<TValue> getValue)
		{
			TValue value;
			if (!map.TryGetValue (key, out value)) 
			{
				value = getValue ();
				map[key] = value;
			}
			return value;
		}

		public static TEntity Remember<TEntity>(this TEntity entity, IGame game, 
			Action<TEntity> resetEntity) where TEntity : class, IEntity
		{
			resetEntity(entity);
			game.Events.OnSavedGameLoad.Subscribe((sender, e) =>
			{
				entity = game.Find<TEntity>(entity.ID);
				resetEntity(entity);
			});
			return entity;
		}
	}
}

