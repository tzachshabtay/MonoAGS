using System;
using AGS.API;

using System.Collections.Generic;
using AreaKey = System.Tuple<AGS.API.IArea, AGS.API.IBitmap>;

namespace AGS.Engine
{
	public class AGSWalkBehindsMap
	{
		private readonly Dictionary<AreaKey, IImage> _images;
		private readonly Dictionary<AreaKey, IObject> _objects;
		private readonly IGameFactory _factory;

		public AGSWalkBehindsMap(IGameFactory factory)
		{
			_factory = factory;
			_images = new Dictionary<AreaKey, IImage> (100);
			_objects = new Dictionary<AreaKey, IObject> (100);
		}

        public IObject GetDrawable(IArea area, IBitmap bg)
		{
            IWalkBehindArea walkBehind = area.GetComponent<IWalkBehindArea>();
            if (walkBehind == null) return null;
			AreaKey key = new AreaKey (area, bg);
			IObject obj = _objects.GetOrAdd(key, () => createObject());
			obj.Z = walkBehind.Baseline == null ? area.Mask.MinY : walkBehind.Baseline.Value;
			obj.Image = _images.GetOrAdd(key, () => createImage(area, bg));
			return obj;
		}

		private IImage createImage(IArea area, IBitmap bg)
		{
			var bitmap = bg.ApplyArea(area);
			return _factory.Graphics.LoadImage(bitmap); 
		}

		private IObject createObject()
		{
			var obj = _factory.Object.GetObject("Walk-Behind Drawable");
			obj.Anchor = new PointF ();
			return obj;
		}
	}
}

