using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesTweenPanel : IFeaturesPanel
    {
        private IComboBox _targetCombobox, _tweenCombobox, _easeCombobox, _repeatCombobox;
        private IButton _addTweenButton, _clearTweensButton;
        private IGame _game;
        private IObject _parent;
        private IPanel _tweensListPanel, _window;
        private readonly Dictionary<string, List<Tuple<string, Func<Tween>>>> _tweens;
        private readonly Dictionary<string, Func<float, float>> _eases;
        private readonly List<RunningTween> _runningTweens;

        public FeaturesTweenPanel(IGame game, IObject parent, IPanel window)
        {
            _window = window;
            _game = game;
            _parent = parent;
            _runningTweens = new List<RunningTween>();
            const float time = 5f;

            var player = game.State.Player;
            var viewport = game.State.Viewport;

            Type easesType = typeof(Ease);
            var easeMethods = easesType.GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static);
            _eases = easeMethods.ToDictionary(k => k.Name, v => (Func<float, float>)v.CreateDelegate(typeof(Func<float, float>)));

            Func<Func<float, float>> ease = () => _eases[_easeCombobox.DropDownPanelList.SelectedItem.Text];

            Func<IObject, List<Tuple<string, Func<Tween>>>> getObjTweens = o =>
            {
                return new List<Tuple<string, Func<Tween>>>{
                    new Tuple<string, Func<Tween>>( "X", () => o.TweenX(100f, time, ease())),
                    new Tuple<string, Func<Tween>>( "Y", () => o.TweenY(100f, time, ease())),
                    new Tuple<string, Func<Tween>>( "Angle", () => o.TweenAngle(45f, time, ease())),
                    new Tuple<string, Func<Tween>>( "Opacity", () => o.TweenOpacity(0, time, ease())),
                    new Tuple<string, Func<Tween>>( "Red", () => o.TweenRed(0, time, ease())),
                    new Tuple<string, Func<Tween>>( "Green", () => o.TweenGreen(0, time, ease())),
                    new Tuple<string, Func<Tween>>( "Blue", () => o.TweenBlue(0, time, ease())),
                    new Tuple<string, Func<Tween>>( "Hue", () => o.TweenHue(360, time, ease())),
                    new Tuple<string, Func<Tween>>( "Saturation", () => o.TweenSaturation(1f, time, ease())),
                    new Tuple<string, Func<Tween>>( "Lightness", () => o.TweenLightness(0f, time, ease())),
                    new Tuple<string, Func<Tween>>( "ScaleX", () => o.TweenScaleX(2f, time, ease())),
                    new Tuple<string, Func<Tween>>( "ScaleY", () => o.TweenScaleY(2f, time, ease())),
                    new Tuple<string, Func<Tween>>( "AnchorX", () => o.TweenAnchorX(1f, time, ease())),
                    new Tuple<string, Func<Tween>>( "AnchorY", () => o.TweenAnchorY(1f, time, ease())),

                };
            };

            Func<IObject, ICropSelfComponent> getCrop = o =>
            {
                var crop = o.GetComponent<ICropSelfComponent>();
                if (crop != null) return crop;
                crop = o.AddComponent<ICropSelfComponent>();
                crop.CropArea = new RectangleF(0f, 0f, o.Width, o.Height);
                return crop;
            };

            Func<IObject, List<Tuple<string, Func<Tween>>>> getObjWithTextureTweens = o =>
            {
                var list = getObjTweens(o);
                list.Add(new Tuple<string, Func<Tween>>("TextureX", () => { setTextureWrap(o); return o.AddComponent<ITextureOffsetComponent>().TweenX(3f, time, ease()); }));
                list.Add(new Tuple<string, Func<Tween>>("TextureY", () => { setTextureWrap(o); return o.AddComponent<ITextureOffsetComponent>().TweenY(3f, time, ease()); }));
                list.Add(new Tuple<string, Func<Tween>>("CropX", () => getCrop(o).TweenX(15f, time, ease())));
                list.Add(new Tuple<string, Func<Tween>>("CropY", () => getCrop(o).TweenY(15f, time, ease())));
                list.Add(new Tuple<string, Func<Tween>>("CropWidth", () => getCrop(o).TweenWidth(0f, time, ease())));
                list.Add(new Tuple<string, Func<Tween>>("CropHeight", () => getCrop(o).TweenHeight(0f, time, ease())));
                return list;
            };

            Func<ISound> getMusic = () =>
            {
                var clip = _game.State.Room.MusicOnLoad;
                if (clip == null) return null;
                return clip.CurrentlyPlayingSounds.FirstOrDefault();
            };

            _tweens = new Dictionary<string, List<Tuple<string, Func<Tween>>>>
            {
                { "Button", getObjWithTextureTweens(TopBar.InventoryButton) },
                { "Character", getObjWithTextureTweens(player) },
                { "Window", getObjTweens(_window) },
                { "Viewport", new List<Tuple<string, Func<Tween>>>{
                        new Tuple<string, Func<Tween>>( "X", () => viewport.TweenX(100f, time, ease())),
                        new Tuple<string, Func<Tween>>( "Y", () => viewport.TweenY(100f, time, ease())),
                        new Tuple<string, Func<Tween>>( "Angle", () => viewport.TweenAngle(45f, time, ease())),
                        new Tuple<string, Func<Tween>>( "ScaleX", () => viewport.TweenScaleX(2f, time, ease())),
                        new Tuple<string, Func<Tween>>( "ScaleY", () => viewport.TweenScaleY(2f, time, ease())),
                        new Tuple<string, Func<Tween>>( "ProjectX", () => viewport.TweenProjectX(100f, time, ease())),
                        new Tuple<string, Func<Tween>>( "ProjectY", () => viewport.TweenProjectY(100f, time, ease())),
                        new Tuple<string, Func<Tween>>( "ProjectWidth", () => viewport.TweenProjectWidth(100f, time, ease())),
                        new Tuple<string, Func<Tween>>( "ProjectHeight", () => viewport.TweenProjectHeight(100f, time, ease())),
                    }
                },
                { "Music", new List<Tuple<string, Func<Tween>>>{
                        new Tuple<string, Func<Tween>>( "Volume", () => { var music = getMusic(); if (music != null) return music.TweenVolume(0f, time, ease()); else return null;}),
                        new Tuple<string, Func<Tween>>( "Pitch", () => { var music = getMusic(); if (music != null) return music.TweenPitch(2f, time, ease()); else return null;}),
                        new Tuple<string, Func<Tween>>( "Panning", () => { var music = getMusic(); if (music != null) return music.TweenPanning(-1f, time, ease()); else return null;}),
                    }
                },
            };

            var y = _parent.Height - 60f;
            _targetCombobox = addCombobox("FeaturesTweenTargetComboBox", 40f, y, "Select Target", onTargetSelected, _tweens.Keys.ToArray());
            _easeCombobox = addCombobox("FeaturesTweenEaseComboBox", 320f, y, "Select Ease", onItemSelected, _eases.Keys.ToArray());
            _tweenCombobox = addCombobox("FeaturesTweenTweenComboBox", 40f, y - 50f, "Select Tween", onItemSelected);
            _tweenCombobox.TextBox.Opacity = 100;
            _tweenCombobox.DropDownButton.Enabled = false;
            _repeatCombobox = addCombobox("FeaturesTweenRepeatComboBox", 320f, y - 50f, "Select Repeat", onItemSelected, Enum.GetValues(typeof(LoopingStyle)).Cast<LoopingStyle>().Select(l => l.ToString()).ToArray());
            _addTweenButton = addButton("FeaturesTweenAddButton", "Add Tween", 250f, y - 120f, addTween);
            _addTweenButton.Enabled = false;
            _addTweenButton.Opacity = 100;
            _clearTweensButton = addButton("FeaturesTweenClearAll", "Clear All", _addTweenButton.X, 20f, clearTweens);
            _tweensListPanel = _game.Factory.UI.GetPanel("FeaturesTweenListPanel", 300f, 300f, 0f, _parent.Height - 250f, parent, false);
            _tweensListPanel.Tint = Colors.Transparent;
            _tweensListPanel.Anchor = new PointF(0f, 1f);
            _tweensListPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            var layout = _tweensListPanel.AddComponent<IStackLayoutComponent>();
            layout.Direction = LayoutDirection.Vertical;
            layout.AbsoluteSpacing = -10f;
            layout.StartLocation = 300f;
            layout.StartLayout();
        }

        public void Close()
        {
            clearTweens();
            removeComboboxFromUI(_targetCombobox);
            removeComboboxFromUI(_tweenCombobox);
            removeComboboxFromUI(_easeCombobox);
            removeComboboxFromUI(_repeatCombobox);
            _game.State.UI.Remove(_addTweenButton);
            _game.State.UI.Remove(_clearTweensButton);
            _game.State.UI.Remove(_tweensListPanel);
            _game.State.Viewport.Camera.Enabled = true;
        }

        public void Show()
        {
            addComboboxToUI(_targetCombobox);
            addComboboxToUI(_tweenCombobox);
            addComboboxToUI(_easeCombobox);
            addComboboxToUI(_repeatCombobox);
            _game.State.UI.Add(_addTweenButton);
            _game.State.UI.Add(_clearTweensButton);
            _game.State.UI.Add(_tweensListPanel);
            _game.State.Viewport.Camera.Enabled = false;
        }

        private void addTween()
        {
            LoopingStyle looping = (LoopingStyle)Enum.Parse(typeof(LoopingStyle),getSelection(_repeatCombobox));
            var targetText = getSelection(_targetCombobox);
            var tweenText = getSelection(_tweenCombobox);
            var targetTweens = _tweens[targetText];
            var tween = targetTweens.First(t => t.Item1 == tweenText).Item2();
            if (tween == null)
            {
                AGSMessageBox.Display("Can't play the tween (perhaps there is no music playing currently?)");
                return;
            }
            tween = tween.RepeatForever(looping, 3f);
            _runningTweens.Add(new RunningTween(string.Format("{0}.{1}", targetText, tweenText), 
                                                tween, _game, _tweensListPanel));
        }

        private void clearTweens()
        {
            foreach (var tween in _runningTweens)
            {
                tween.Stop();
            }
            _runningTweens.Clear();
        }

        private void onTargetSelected(ListboxItemArgs args)
        {
            var list = _tweenCombobox.DropDownPanelList;
            list.Items.Clear();
            list.Items.AddRange(_tweens[getSelection(_targetCombobox)]
                                .Select(k => new AGSStringItem { Text = k.Item1}).Cast<IStringItem>().ToList());
            if (!_tweenCombobox.DropDownButton.Enabled)
            {
                _tweenCombobox.DropDownButton.Enabled = true;
                _tweenCombobox.TextBox.TweenOpacity(0, 0.5f);
            }
            else
            {
                var item = list.Items.FirstOrDefault(i => i.Text == _tweenCombobox.TextBox.Text);
                if (item != null)
                {
                    list.SelectedIndex = list.Items.IndexOf(item);
                }
                else
                {
                    _tweenCombobox.DropDownPanelList.SelectedIndex = -1;
                    _tweenCombobox.TextBox.Text = "Select Tween";
                    _addTweenButton.Enabled = false;
                    _addTweenButton.TweenOpacity(100, 0.5f);
                }
            }
            onItemSelected(args);
        }

        private void onItemSelected(ListboxItemArgs args)
        {
            if (hasSelection(_tweenCombobox) && hasSelection(_targetCombobox) &&
                hasSelection(_easeCombobox) && hasSelection(_repeatCombobox) && !_addTweenButton.Enabled)
            {
                _addTweenButton.Enabled = true;
                _addTweenButton.TweenOpacity(0, 0.5f);
            }
        }

        private IButton addButton(string id, string text, float x, float y, Action onClick)
        {
            AGSTextConfig idleConfig = new AGSTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.White));
            AGSTextConfig hoverConfig = new AGSTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.Black));
            var idle = new ButtonAnimation(AGSBorders.SolidColor(Colors.AliceBlue, 5f), idleConfig, Colors.Transparent);
            var hovered = new ButtonAnimation(AGSBorders.SolidColor(Colors.Goldenrod, 5f), hoverConfig, Colors.Yellow);
            var pushed = new ButtonAnimation(AGSBorders.SolidColor(Colors.AliceBlue, 7f), idleConfig, Colors.Transparent);
            var button = _game.Factory.UI.GetButton(id, idle, hovered, pushed, x, y, _parent, text, null, false, 150f, 50f);
            button.MouseClicked.Subscribe(m => onClick());
            return button;
        }

        private IComboBox addCombobox(string id, float x, float y, string initialText, Action<ListboxItemArgs> callback, params string[] options)
        {
            var combo = _game.Factory.UI.GetComboBox(id, null, null, null, _parent, false, 220f);
            combo.TextBox.Text = initialText;
            combo.X = x;
            combo.Y = y;
            var list = combo.DropDownPanelList;
            list.Items.AddRange(options.Select(o => (IStringItem)new AGSStringItem { Text = o }).ToList());

            if (callback != null) list.OnSelectedItemChanged.Subscribe(callback);
            return combo;
        }

        private bool hasSelection(IComboBox combo)
        {
            return combo.DropDownPanelList.SelectedItem != null;
        }

        private string getSelection(IComboBox combo)
        {
            return combo.DropDownPanelList.SelectedItem.Text;
        }

        private void addComboboxToUI(IComboBox combo)
        {
            _game.State.UI.Add(combo.TextBox);
            _game.State.UI.Add(combo.DropDownButton);
            _game.State.UI.Add(combo);
        }

        private void removeComboboxFromUI(IComboBox combo)
        {
            _game.State.UI.Remove(combo.TextBox);
            _game.State.UI.Remove(combo.DropDownButton);
            _game.State.UI.Remove(combo);
        }

        private void setTextureWrap(IObject obj)
        {
            var animation = obj.Animation;
            if (animation == null) return;
            foreach (var frame in animation.Frames)
            {
                frame.Sprite.Image.Texture.Config = new AGSTextureConfig(ScaleDownFilters.Nearest, ScaleUpFilters.Nearest, 
                                                                         TextureWrap.Repeat, TextureWrap.Repeat);
            }
        }

        private class RunningTween
        {
            private Tween _tween;
            private static int id;
            private IPanel _panel;
            private ISlider _slider;
            private IButton _playPauseButton, _rewindButton, _stopButton;
            private ILabel _label;
            private IGame _game;

            public RunningTween(string name, Tween tween, IGame game, IObject parent)
            {
                _game = game;
                _tween = tween;
                var factory = game.Factory.UI;
                var tweenId = ++id;
                _panel = factory.GetPanel(string.Format("FeaturesTweenPanel_{0}", tweenId), 300f, 20f, 0f, 0f, parent);
                _panel.Tint = Colors.Transparent;

                _slider = factory.GetSlider(string.Format("FeaturesTweenSlider_{0}", tweenId), null, null, 0f, 0f, tween.DurationInTicks, _panel);
                _slider.Location = new AGSLocation(20f, 10f);
                _slider.HandleGraphics.Anchor = new PointF(0.5f, 0.5f);
                _slider.Direction = SliderDirection.LeftToRight;
                _slider.Graphics.Anchor = new PointF(0f, 0.5f);
                _slider.Graphics.Image = new EmptyImage(_panel.Width - 100f, 10f);
                _slider.Graphics.Border = AGSBorders.SolidColor(Colors.DarkGray, 0.5f, true);
                _slider.HandleGraphics.Border = AGSBorders.SolidColor(Colors.White, 0.5f, true);
                HoverEffect.Add(_slider.Graphics, Colors.Green, Colors.LightGray);
                HoverEffect.Add(_slider.HandleGraphics, Colors.DarkGreen, Colors.WhiteSmoke);

                _label = factory.GetLabel(string.Format("FeaturesTweenLabel_{0}", tweenId), name, 100f, 20f,
                                          _slider.X + _slider.Graphics.Width / 2f, _slider.Y + 10f, _panel,
                                          new AGSTextConfig(autoFit: AutoFit.TextShouldFitLabel));
                _label.Anchor = new PointF(0.5f, 0f);

                AGSTextConfig idleConfig = new AGSTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.White));
                AGSTextConfig hoverConfig = new AGSTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.Black));

                var idle = new ButtonAnimation(AGSBorders.SolidColor(Colors.AliceBlue, 2f), idleConfig, Colors.Transparent);
                var hovered = new ButtonAnimation(AGSBorders.SolidColor(Colors.Goldenrod, 2f), hoverConfig, Colors.Yellow);
                var pushed = new ButtonAnimation(AGSBorders.SolidColor(Colors.AliceBlue, 4f), idleConfig, Colors.Transparent);

                _rewindButton = factory.GetButton(string.Format("FeaturesTweenRewindButton_{0}", tweenId), idle, hovered, pushed, 270f, 0f, _panel, "Rewind", width: 100f, height: 30f);
                _playPauseButton = factory.GetButton(string.Format("FeaturesTweenPlayPauseButton_{0}", tweenId), idle, hovered, pushed, 380f, 0f, _panel, "Pause", width: 100f, height: 30f);
                _stopButton = factory.GetButton(string.Format("FeaturesTweenStopButton_{0}", tweenId), idle, hovered, pushed, 490f, 0f, _panel, "Stop", width: 100f, height: 30f);
                _stopButton.MouseClicked.Subscribe(_ => Stop());
                _rewindButton.MouseClicked.Subscribe(_ => _tween.Rewind());
                _playPauseButton.MouseClicked.Subscribe(onPlayPauseClick);
                _slider.OnValueChanging.Subscribe(onSliderValueChanging);
                game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            }

            public void Stop()
            {
                _tween.Stop(TweenCompletion.Rewind);
                _game.Events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
                var ui = _game.State.UI;
                _panel.Visible = false;
                ui.Remove(_panel);
                _panel.TreeNode.SetParent(null);
                ui.Remove(_rewindButton);
                ui.Remove(_stopButton);
                ui.Remove(_playPauseButton);
                ui.Remove(_slider);
                ui.Remove(_slider.Graphics);
                ui.Remove(_slider.HandleGraphics);
                ui.Remove(_label);
            }

            private void onRepeatedlyExecute()
            {
                _slider.Value = _tween.ElapsedTicks;
            }

            private void onPlayPauseClick(MouseButtonEventArgs args)
            {
                if (_playPauseButton.Text == "Pause")
                {
                    _tween.Pause();
                    _playPauseButton.Text = "Resume";
                }
                else
                {
                    _tween.Resume();
                    _playPauseButton.Text = "Pause";
                }
            }

            private void onSliderValueChanging(SliderValueEventArgs args)
            {
                _tween.ElapsedTicks = args.Value;
            }
        }
    }
}
