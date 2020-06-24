﻿using System;
using System.Threading.Tasks;
using AGS.API;
using Autofac;

namespace AGS.Engine
{
	public class AGSRoomFactory : IRoomFactory
	{
        private readonly Resolver _resolver;
        private Lazy<IMaskLoader> _masks;

        public AGSRoomFactory(Resolver resolver)
		{
			_resolver = resolver;
            _masks = new Lazy<IMaskLoader>(resolver.Container.Resolve<IMaskLoader>);
		}

        public IEdge GetEdge(float value = 0f)
		{
			IEdge edge = _resolver.Container.Resolve<IEdge>();
			edge.Value = value;
			return edge;
		}

		public IRoom GetRoom(string id, float leftEdge = 0f, float rightEdge = 0f, float topEdge = 0f, float bottomEdge = 0f)
		{
			AGSEdges edges = new AGSEdges (GetEdge(leftEdge), GetEdge(rightEdge), GetEdge(topEdge), GetEdge(bottomEdge));
			TypedParameter edgeParam = new TypedParameter (typeof(IAGSEdges), edges);
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			IRoom room = _resolver.Container.Resolve<IRoom>(idParam, edgeParam);
			return room;
		}

        public IArea GetArea(string maskPath, IRoom room = null, bool isWalkable = false, bool isWalkBehind = false)
        {
            return createArea(maskPath, _masks.Value.Load(maskPath), room, isWalkable, isWalkBehind);
        }

        public IArea GetArea(string id, IMask mask, IRoom room = null, bool isWalkable = false, bool isWalkBehind = false)
        {
            return createArea(id, mask, room, isWalkable, isWalkBehind);
        }

        public async Task<IArea> GetAreaAsync(string maskPath, IRoom room = null, bool isWalkable = false, bool isWalkBehind = false)
        {
            return createArea(maskPath, await _masks.Value.LoadAsync(maskPath, id: $"Mask:{maskPath}"), room, isWalkable, isWalkBehind);
        }

        public void CreateScaleArea(IArea area, float minScaling, float maxScaling, bool scaleObjectsX = true, bool scaleObjectsY = true, bool scaleVolume = true)
        {
            var component = area.AddComponent<IScalingArea>();
            component.MinScaling = minScaling;
            component.MaxScaling = maxScaling;
            component.ScaleObjectsX = scaleObjectsX;
            component.ScaleObjectsY = scaleObjectsY;
            component.ScaleVolume = scaleVolume;
        }

        public void CreateZoomArea(IArea area, float minZoom, float maxZoom)
        {
            var component = area.AddComponent<IZoomArea>();
            component.MinZoom = minZoom;
            component.MaxZoom = maxZoom;
        }

        [MethodWizard]
        public IArea GetArea(string id, IMask mask, bool isWalkable, bool isWalkbehind, 
             bool isScaling, [MethodParam(Default = 1f)]float minScaling, [MethodParam(Default = 1f)]float maxScaling,
             bool isZoom, [MethodParam(Default = 1f)]float minZoom, [MethodParam(Default = 1f)]float maxZoom, 
             [MethodParam(Browsable = false)] IRoom room = null)
        {
            var area = GetArea(id, mask, room, isWalkable, isWalkbehind);
            if (isScaling) CreateScaleArea(area, minScaling, maxScaling);
            if (isZoom) CreateZoomArea(area, minZoom, maxZoom);
            return area;
        }

        private IArea createArea(string id, IMask mask, IRoom room, bool isWalkable, bool isWalkBehind)
        {
            TypedParameter idParam = new TypedParameter(typeof(string), id);
            IArea area = _resolver.Container.Resolve<IArea>(idParam);
            area.Mask = mask;
            if (isWalkable) area.AddComponent<IWalkableArea>();
            if (isWalkBehind) area.AddComponent<IWalkBehindArea>();
            room?.Areas.Add(area);
            return area;
        }
	}
}

