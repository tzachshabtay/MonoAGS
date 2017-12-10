using AGS.API;
using System.Collections.Generic;
using AreaKey = System.ValueTuple<AGS.API.IArea, AGS.API.IBitmap>;
using System;

namespace AGS.Engine
{
	public class AGSWalkBehindsMap
	{
		private readonly Dictionary<AreaKey, IImage> _images;
		private readonly Dictionary<AreaKey, IObject> _objects;
		private readonly IGameFactory _factory;
        private readonly Func<AreaKey, IImage> _createImageFunc;
        private readonly Func<AreaKey, IObject> _createObjectFunc;

		public AGSWalkBehindsMap(IGameFactory factory)
		{
			_factory = factory;
			_images = new Dictionary<AreaKey, IImage> (100);
			_objects = new Dictionary<AreaKey, IObject> (100);

            //Creating delegates in advance to avoid memory allocations on critical path
            _createImageFunc = key => createImage(key.Item1, key.Item2);
            _createObjectFunc = key => createObject(key.Item1.ID);
		}

        public IObject GetDrawable(IArea area, IBitmap bg)
		{
            IWalkBehindArea walkBehind = area.GetComponent<IWalkBehindArea>();
            if (walkBehind == null) return null;
			AreaKey key = new AreaKey (area, bg);
			IObject obj = _objects.GetOrAdd(key, _createObjectFunc);
			obj.Z = walkBehind.Baseline == null ? area.Mask.MinY : walkBehind.Baseline.Value;
			obj.Image = _images.GetOrAdd(key, _createImageFunc);         
			return obj;
		}

        private IImage createImage(IArea area, IBitmap bg)
		{
            var bitmap = bg.ApplyArea(area);
			return _factory.Graphics.LoadImage(bitmap); 
		}

        private IObject createObject(string id)
		{            
            var obj = _factory.Object.GetObject("Walk-Behind Drawable: " + id);
			obj.Anchor = new PointF ();
			return obj;
		}
	}
}

