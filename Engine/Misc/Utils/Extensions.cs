using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
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

		public static void Clear(this Bitmap bitmap, Color color)
		{
			//todo: Possibly improve performance by using direct access math
			Graphics g = Graphics.FromImage(bitmap);
			g.Clear(color);
		}

		public static void Clear(this Bitmap bitmap)
		{
			bitmap.Clear(Color.White);
		}

		private static Graphics _graphics = Graphics.FromImage(new Bitmap (1, 1));
		public static System.Drawing.SizeF Measure(this string text, Font font, int maxWidth = int.MaxValue)
		{
			_graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			return _graphics.MeasureString(text, font, maxWidth, StringFormat.GenericTypographic);
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

