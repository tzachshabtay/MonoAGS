﻿using System;
using System.Collections.Generic;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IRotateComponent))]
    [RequiredComponent(typeof(IUIEvents))]
    public partial class EntityDesigner : AGSComponent
    {
        private readonly IGameFactory _factory;
        private readonly IGameState _state;
        private readonly IGameEvents _events;
        private readonly IInput _input;
        private readonly List<ResizeHandle> _resizeHandles;
        private readonly List<RotateHandle> _rotateHandles;
        private IBoundingBoxComponent _box;
        private bool _resizeVisible;

        public EntityDesigner(IGameFactory factory, IGameState state, IGameEvents events, IInput input)
        {
            _factory = factory;
            _state = state;
            _events = events;
            _input = input;
            _resizeVisible = true;
            _resizeHandles = new List<ResizeHandle>(8);
            _rotateHandles = new List<RotateHandle>(4);
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            addResizeHandles(entity, Direction.Right, Direction.Left, Direction.Up, Direction.Down,
                             Direction.UpRight, Direction.UpLeft, Direction.DownRight, Direction.DownLeft);
            addRotateHandles(entity, Direction.UpRight, Direction.UpLeft, Direction.DownRight, Direction.DownLeft);

            entity.Bind<IScaleComponent>(c => setScale(c), _ => setScale(null));
            entity.Bind<IRotateComponent>(c => setRotate(c), _ => setRotate(null));

            entity.Bind<IBoundingBoxComponent>(
                c => { _box = c; c.OnBoundingBoxesChanged.Subscribe(onBoundingBoxChanged); updatePositions(); },
                c => { _box = null; c.OnBoundingBoxesChanged.Unsubscribe(onBoundingBoxChanged); });

            entity.Bind<IUIEvents>(c => c.MouseClicked.Subscribe(onMouseClicked), c => c.MouseClicked.Unsubscribe(onMouseClicked));

            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public override void Dispose()
        {
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            foreach (var handle in _resizeHandles) handle.Dispose();
            foreach (var handle in _rotateHandles) handle.Dispose();
        }

        private void onMouseClicked(MouseButtonEventArgs obj)
        {
            _resizeVisible = !_resizeVisible;
            foreach (var handle in _resizeHandles) handle.SetVisible(_resizeVisible);
            foreach (var handle in _rotateHandles) handle.SetVisible(!_resizeVisible);
        }

        private void setScale(IScaleComponent scale)
        {
            foreach (var handle in _resizeHandles) handle.SetScale(scale);
        }

        private void setRotate(IRotateComponent rotate)
        {
            foreach (var handle in _rotateHandles) handle.SetRotate(rotate);
        }

        private void addResizeHandles(IEntity entity, params Direction[] directions)
        {
            var config = FontIcons.IconConfig;
            foreach (var direction in directions)
            {
                var label = _factory.UI.GetLabel($"{entity.ID}_ResizeHandle{direction}", "", 5f, 5f, 0f, 0f, config: config, addToUi: false);
                _resizeHandles.Add(new ResizeHandle(label, _state, _input, direction));
            }
        }

        private void addRotateHandles(IEntity entity, params Direction[] directions)
        {
            var config = FontIcons.IconConfig;
            foreach (var direction in directions)
            {
                var label = _factory.UI.GetLabel($"{entity.ID}_RotateHandle{direction}", "", 5f, 5f, 0f, 0f, config: config, addToUi: false);
                _rotateHandles.Add(new RotateHandle(label, _state, _input, direction));
            }
        }

        private void onRepeatedlyExecute(IRepeatedlyExecuteEventArgs obj)
        {
            foreach (var handle in _resizeHandles) handle.Visit();
            foreach (var handle in _rotateHandles) handle.Visit();
        }

        private void onBoundingBoxChanged()
        {
            updatePositions();
        }

        private void updatePositions()
        {
            var box = _box;
            if (box == null) return;
            foreach (var handle in _resizeHandles) handle.UpdatePosition(box);
            foreach (var handle in _rotateHandles) handle.UpdatePosition(box);
        }
    }
}