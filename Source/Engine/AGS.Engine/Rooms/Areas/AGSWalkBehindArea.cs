using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
	public class AGSWalkBehindArea : AGSComponent, IWalkBehindArea
	{
        private readonly IGameState _state;
        private readonly Dictionary<IBitmap, IImage> _images;
        private readonly Func<IBitmap, IImage> _createImageFunc;
        private readonly IGameFactory _factory;
        private IRoom _room;
        private IEntity _entity;
        private IAreaComponent _area;
        private IObject _drawable;

        public AGSWalkBehindArea(IGameState state, IGameFactory factory)
        {
            _state = state;
            _factory = factory;
            _createImageFunc = createImage;
            _images = new Dictionary<IBitmap, IImage>();
        }

		#region IWalkBehindArea implementation

		public float? Baseline { get; set; }

		#endregion

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IAreaComponent>(c => _area = c, _ => _area = null);
            _entity = entity;
            refreshRoom();
            _state.Rooms?.OnListChanged?.Subscribe(onRoomsChanged);
        }

        private void onRoomsChanged(AGSListChangedEventArgs<IRoom> args)
        {
            if (_room == null && args.ChangeType == ListChangeType.Add) refreshRoom();
        }

        private void refreshRoom()
        {
            unsubscribeRoom();
            _room = _state?.Rooms?.FirstOrDefault(r => r.Areas.Contains(_entity));
            subscribeRoom();
        }

        private void subscribeRoom()
        {
            updateObject();
            _room?.Areas.OnListChanged.Subscribe(onAreasChanged);
            var image = _room?.Background?.GetComponent<IImageComponent>();
            if (image == null) return;
            image.PropertyChanged += onBackgroundPropertyChanged;
        }

        private void unsubscribeRoom()
        {
            if (_drawable != null) _room?.Objects?.Remove(_drawable);
            _room?.Areas.OnListChanged.Unsubscribe(onAreasChanged);
            var image = _room?.Background?.GetComponent<IImageComponent>();
            if (image == null) return;
            image.PropertyChanged -= onBackgroundPropertyChanged;
        }

        private void onAreasChanged(AGSListChangedEventArgs<IArea> args)
        {
            refreshRoom();
        }

        private void onBackgroundPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IImageComponent.Image)) return;
            updateObject();
        }

        private void updateObject()
        {
            var bitmap = _room?.Background?.Image?.OriginalBitmap;
            if (bitmap == null) return;
            _drawable = _drawable ?? createObject();
            _drawable.Image = _images.GetOrAdd(bitmap, _createImageFunc);
            _room?.Objects?.Add(_drawable);
        }

        private IObject createObject()
        {
            var obj = _factory.Object.GetObject($"Walk-Behind Drawable: {_entity.ID}");
            obj.Pivot = new PointF();
            obj.Z = Baseline == null ? _area.Mask.MinY : Baseline.Value;
            obj.Enabled = false;
            return obj;
        }

        private IImage createImage(IBitmap bg)
        {
            var bitmap = bg.ApplyArea(_area);
            return _factory.Graphics.LoadImage(bitmap);
        }
    }
}