using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class UIEventsAggregator : IDisposable
    {
        private class Subscriber
        {
            public Subscriber(IEntity entity, Action<bool> setMouseIn, IUIEvents events, IEnabledComponent enabled, IVisibleComponent visible)
            {
                Entity = entity;
                SetMouseIn = setMouseIn;
                Events = events;
                Enabled = enabled;
                Visible = visible;

                LeftMouseClickTimer = new Stopwatch();
                RightMouseClickTimer = new Stopwatch();
                LeftMouseDoubleClickTimer = new Stopwatch();
                RightMouseDoubleClickTimer = new Stopwatch();
            }

            public IEntity Entity { get; private set; }
            public IUIEvents Events { get; private set; }
            public IEnabledComponent Enabled { get; private set; }
            public IVisibleComponent Visible { get; private set; }
            public Action<bool> SetMouseIn { get; private set; }

            public bool FireMouseMove { get; set; }
            public bool FireMouseEnter { get; set; }
            public bool FireMouseLeave { get; set; }
            public bool IsFocused { get; set; }
            public bool WasClickedIn { get; set; }

            public Stopwatch LeftMouseClickTimer, RightMouseClickTimer, LeftMouseDoubleClickTimer, RightMouseDoubleClickTimer;
        }

        private readonly IInput _input;
        private readonly IHitTest _hitTest;
        private readonly IGameEvents _gameEvents;
        private readonly IFocusedUI _focus;
        private List<Subscriber> _subscribers;
        private readonly ConcurrentQueue<Subscriber> _subscribersToAdd;
        private readonly ConcurrentQueue<string> _subscribersToRemove;
        private MousePosition _mousePosition;
        private bool _leftMouseDown, _rightMouseDown;
        private int _inUpdate; //For preventing re-entrancy

        public UIEventsAggregator(IInput input, IHitTest hitTest, IGameEvents gameEvents, IFocusedUI focus)
        {
            _hitTest = hitTest;
            _focus = focus;
            _subscribersToAdd = new ConcurrentQueue<Subscriber>();
            _subscribersToRemove = new ConcurrentQueue<string>();
            _input = input;
            _gameEvents = gameEvents;
            _subscribers = new List<Subscriber>(100);
            gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public void Dispose()
        {
            _gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            _subscribers = null;
        }

        public void Subscribe(IEntity entity, Action<bool> setMouseIn, IUIEvents uiEvents, IEnabledComponent enabled, IVisibleComponent visible)
        {
            _subscribersToAdd.Enqueue(new Subscriber(entity, setMouseIn, uiEvents, enabled, visible));
        }

        public void Unsubscribe(IEntity entity)
        {
            if (entity == null) return;
            _subscribersToRemove.Enqueue(entity.ID);
        }

        private void onRepeatedlyExecute()
        {
            if (Interlocked.CompareExchange(ref _inUpdate, 1, 0) != 0) return;
            try
            {
                MousePosition position = _input.MousePosition;

                var obj = _hitTest.ObjectAtMousePosition;
                bool leftMouseDown = _input.LeftMouseButtonDown;
                bool rightMouseDown = _input.RightMouseButtonDown;
                bool anyButtonDown = leftMouseDown || rightMouseDown;
                var subscribers = _subscribers;

                Subscriber subscriberToAdd;
                while (_subscribersToAdd.TryDequeue(out subscriberToAdd)) subscribers.Add(subscriberToAdd);

                string entityToRemove;
                while (_subscribersToRemove.TryDequeue(out entityToRemove))
                {
                    int index = subscribers.FindIndex(sub => sub.Entity.ID == entityToRemove);
                    if (index >= 0) subscribers.RemoveAt(index);
                }

                foreach (var subscriber in subscribers)
                {
                    if (!subscriber.Enabled.Enabled || !subscriber.Visible.Visible || subscriber.Enabled.ClickThrough) continue;
                    bool mouseIn = obj == subscriber.Entity;

                    subscriber.FireMouseMove = mouseIn && !_mousePosition.Equals(position);
                    subscriber.FireMouseEnter = mouseIn && !subscriber.Events.IsMouseIn;
                    subscriber.FireMouseLeave = !mouseIn && subscriber.Events.IsMouseIn;
                    subscriber.SetMouseIn(mouseIn);
                    if (anyButtonDown) 
                    {
                        subscriber.WasClickedIn = mouseIn && anyButtonDown;
                    }
                }

                _mousePosition = position;

                bool wasLeftMouseDown = _leftMouseDown;
                bool wasRightMouseDown = _rightMouseDown;
                _leftMouseDown = leftMouseDown;
                _rightMouseDown = rightMouseDown;

                foreach (var subscriber in subscribers)
                {
                    if (!subscriber.Enabled.Enabled || !subscriber.Visible.Visible || subscriber.Enabled.ClickThrough) continue;
                    fireAndForgetEvents(subscriber, position, wasLeftMouseDown, wasRightMouseDown, leftMouseDown, rightMouseDown);
                }
            }
            finally
            {
                _inUpdate = 0;
            }
        }

        //We can't await the events, as inside the events somebody might block waiting for a UI event,
        //for example call AGSMessageBox.YesNo dialog. As we have the _inUpdate variable for preventing re-entrancy,
        //we'll have a race condition.
        private async void fireAndForgetEvents(Subscriber subscriber, MousePosition position, bool wasLeftMouseDown, bool wasRightMouseDown, bool leftMouseDown, bool rightMouseDown)
        {
            await handleMouseButton(subscriber, subscriber.LeftMouseClickTimer, subscriber.LeftMouseDoubleClickTimer, wasLeftMouseDown, leftMouseDown, MouseButton.Left);
            await handleMouseButton(subscriber, subscriber.RightMouseClickTimer, subscriber.RightMouseDoubleClickTimer, wasRightMouseDown, rightMouseDown, MouseButton.Right);

            if (subscriber.FireMouseEnter) await subscriber.Events.MouseEnter.InvokeAsync(new MousePositionEventArgs(position));
            else if (subscriber.FireMouseLeave) await subscriber.Events.MouseLeave.InvokeAsync(new MousePositionEventArgs(position));
            if (subscriber.FireMouseMove) await subscriber.Events.MouseMove.InvokeAsync(new MousePositionEventArgs(position));
        }

        private async Task handleMouseButton(Subscriber subscriber, Stopwatch sw, Stopwatch doubleClickSw, bool wasDown, bool isDown, MouseButton button)
        {
            bool fireDown = !wasDown && isDown && subscriber.Events.IsMouseIn;
            bool fireDownOutside = !wasDown && isDown && !subscriber.Events.IsMouseIn && subscriber.IsFocused;
            subscriber.IsFocused = fireDown || _focus.FocusedWindow == subscriber.Entity || _focus.HasKeyboardFocus == subscriber.Entity;
            bool fireUp = wasDown && !isDown && subscriber.WasClickedIn;
            if (fireDown)
            {
                sw.Restart();
            }
            bool fireClick = false;
            bool fireDoubleClick = false;
            if (fireUp)
            {
                if (subscriber.Events.IsMouseIn && sw.ElapsedMilliseconds < 1500 && sw.ElapsedMilliseconds != 0)
                {
                    fireClick = true;
                    if (doubleClickSw.ElapsedMilliseconds == 0)
                    {
                        doubleClickSw.Restart();
                    }
                    else
                    {
                        if (doubleClickSw.ElapsedMilliseconds < 1500)
                        {
                            fireDoubleClick = true;
                        }
                        doubleClickSw.Stop();
                        doubleClickSw.Reset();
                    }
                }
                sw.Stop();
                sw.Reset();
            }

            if (fireDown || fireUp || fireClick || fireDownOutside)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(subscriber.Entity, button, _mousePosition);
                if (fireDown) await subscriber.Events.MouseDown.InvokeAsync(args);
                else if (fireUp) await subscriber.Events.MouseUp.InvokeAsync(args);
                else if (fireDownOutside) await subscriber.Events.LostFocus.InvokeAsync(args);
                if (fireClick) await subscriber.Events.MouseClicked.InvokeAsync(args);
                if (fireDoubleClick) await subscriber.Events.MouseDoubleClicked.InvokeAsync(args);
            }
        }
    }
}
