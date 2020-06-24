﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private IPanel _tweensListScrollingPanel, _tweensListContentsPanel, _window;
        private readonly Dictionary<string, List<(string propertyName, Func<Tween> getTween)>> _tweens;
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
            Dictionary<string, Func<float, float>> eases = easeMethods.ToDictionary(k => k.Name, v => (Func<float, float>)v.CreateDelegate(typeof(Func<float, float>)));

            Func<Func<float, float>> ease = () => eases[_easeCombobox.DropDownPanelList.SelectedItem.Text];

            Func<IObject, List<(string, Func<Tween>)>> getObjTweens = o =>
            {
                return new List<(string, Func<Tween>)>{
                    ( "X", () => o.TweenX(100f, time, ease())),
                    ( "Y", () => o.TweenY(100f, time, ease())),
                    ( "Angle", () => o.TweenAngle(45f, time, ease())),
                    ( "Opacity", () => o.TweenOpacity(0, time, ease())),
                    ( "Red", () => o.TweenRed(0, time, ease())),
                    ( "Green", () => o.TweenGreen(255, time, ease())),
                    ( "Blue", () => o.TweenBlue(255, time, ease())),
                    ( "Brightness", () => o.TweenBrightness(3f, time, ease())),
                    ( "Tint Hue", () => o.TweenTintHue(360, time, ease())),
                    ( "Tint Saturation", () => o.TweenTintSaturation(0f, time, ease())),
                    ( "Tint Lightness", () => o.TweenTintLightness(0f, time, ease())),
                    ( "Saturation", () => o.AddComponent<ISaturationEffectComponent>().TweenSaturation(0f, time, ease())),
                    ( "ScaleX", () => o.TweenScaleX(2f, time, ease())),
                    ( "ScaleY", () => o.TweenScaleY(2f, time, ease())),
                    ( "PivotX", () => o.TweenPivotX(1f, time, ease())),
                    ( "PivotY", () => o.TweenPivotY(1f, time, ease())),

                };
            };

            Func<IObject, ICropSelfComponent> getCrop = o =>
            {
                var crop = o.GetComponent<ICropSelfComponent>();
                if (crop != null) return crop;
                crop = o.AddComponent<ICropSelfComponent>();
                crop.CropArea = (0f, 0f, o.Width, o.Height);
                return crop;
            };

            Func<IObject, List<(string, Func<Tween>)>> getObjWithTextureTweens = o =>
            {
                var list = getObjTweens(o);
                list.Add(("TextureX", () => { setTextureWrap(o); return o.AddComponent<ITextureOffsetComponent>().TweenX(3f, time, ease()); }));
                list.Add(("TextureY", () => { setTextureWrap(o); return o.AddComponent<ITextureOffsetComponent>().TweenY(3f, time, ease()); }));
                list.Add(("CropX", () => getCrop(o).TweenX(15f, time, ease())));
                list.Add(("CropY", () => getCrop(o).TweenY(15f, time, ease())));
                list.Add(("CropWidth", () => getCrop(o).TweenWidth(0f, time, ease())));
                list.Add(("CropHeight", () => getCrop(o).TweenHeight(0f, time, ease())));
                return list;
            };

            Func<ISound> getMusic = () =>
            {
                var clip = _game.State.Room.MusicOnLoad;
                if (clip == null) return null;
                return clip.CurrentlyPlayingSounds.FirstOrDefault();
            };

            _tweens = new Dictionary<string, List<(string, Func<Tween>)>>
            {
                { "Button", getObjWithTextureTweens(TopBar.InventoryButton) },
                { "Character", getObjWithTextureTweens(player) },
                { "Window", getObjTweens(_window) },
                { "Viewport", new List<(string, Func<Tween>)>{
                        ( "X", () => viewport.TweenX(100f, time, ease())),
                        ( "Y", () => viewport.TweenY(100f, time, ease())),
                        ( "Angle", () => viewport.TweenAngle(45f, time, ease())),
                        ( "ScaleX", () => viewport.TweenScaleX(2f, time, ease())),
                        ( "ScaleY", () => viewport.TweenScaleY(2f, time, ease())),
                        ( "ProjectX", () => viewport.TweenProjectX(0.5f, time, ease())),
                        ( "ProjectY", () => viewport.TweenProjectY(0.5f, time, ease())),
                        ( "ProjectWidth", () => viewport.TweenProjectWidth(0.5f, time, ease())),
                        ( "ProjectHeight", () => viewport.TweenProjectHeight(0.5f, time, ease())),
                        ( "PivotX", () => viewport.TweenPivotX(0.5f, time, ease())),
                        ( "PivotY", () => viewport.TweenPivotY(0.5f, time, ease())),
                        ( "Saturation", () => //maybe we should a "Screen" target and put the saturation tween there
                            {
                                SaturationEffectComponent effect = new SaturationEffectComponent(game.Factory.Shaders, game.Events);
                                effect.Init(null, typeof(SaturationEffectComponent));
                                return effect.TweenSaturation(0f, time, ease());
                        })
                    }
                },
                { "Music", new List<(string, Func<Tween>)>{
                        ( "Volume", () => { var music = getMusic(); return music?.TweenVolume(0f, time, ease());}),
                        ( "Pitch", () => { var music = getMusic(); return music?.TweenPitch(2f, time, ease());}),
                        ( "Panning", () => { var music = getMusic(); return music?.TweenPanning(-1f, time, ease());}),
                    }
                },
            };

            var y = _parent.Height - 60f;
            _targetCombobox = addCombobox("FeaturesTweenTargetComboBox", 40f, y, "Select Target", onTargetSelected, _tweens.Keys.ToArray());
            _easeCombobox = addCombobox("FeaturesTweenEaseComboBox", 320f, y, "Select Ease", onItemSelected, eases.Keys.ToArray());
            _easeCombobox.SuggestMode = ComboSuggest.Enforce;
            _tweenCombobox = addCombobox("FeaturesTweenTweenComboBox", 40f, y - 50f, "Select Tween", onItemSelected);
            _tweenCombobox.TextBox.Opacity = 100;
            _tweenCombobox.DropDownButton.Enabled = false;
            _tweenCombobox.SuggestMode = ComboSuggest.Enforce;
            _repeatCombobox = addCombobox("FeaturesTweenRepeatComboBox", 320f, y - 50f, "Select Repeat", onItemSelected, Enum.GetValues(typeof(LoopingStyle)).Cast<LoopingStyle>().Select(l => l.ToString()).ToArray());
            _addTweenButton = addButton("FeaturesTweenAddButton", "Add Tween", 250f, y - 120f, addTween);
            _addTweenButton.Enabled = false;
            _addTweenButton.Opacity = 100;
            _clearTweensButton = addButton("FeaturesTweenClearAll", "Clear All", _addTweenButton.X, 20f, clearTweens);
            _tweensListScrollingPanel = _game.Factory.UI.GetPanel("FeaturesTweenListPanel", 580f, 250f, 0f, _parent.Height - 200f, parent, false);
            _tweensListScrollingPanel.Tint = Colors.Transparent;
            _tweensListContentsPanel = _game.Factory.UI.CreateScrollingPanel(_tweensListScrollingPanel);
            _tweensListContentsPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            _tweensListScrollingPanel.Pivot = (0f, 1f);
            var layout = _tweensListContentsPanel.AddComponent<IStackLayoutComponent>();
            layout.Direction = LayoutDirection.Vertical;
            layout.AbsoluteSpacing = -10f;
            layout.StartLocation = _tweensListContentsPanel.Height - 20f;
            layout.StartLayout();
        }

        public Task Close()
        {
            clearTweens();
            removeComboboxFromUI(_targetCombobox);
            removeComboboxFromUI(_tweenCombobox);
            removeComboboxFromUI(_easeCombobox);
            removeComboboxFromUI(_repeatCombobox);
            _game.State.UI.Remove(_addTweenButton);
            _game.State.UI.Remove(_clearTweensButton);
            _game.State.UI.Remove(_tweensListScrollingPanel);
            _game.State.UI.Remove(_tweensListContentsPanel);
            _game.State.Viewport.Camera.Enabled = true;
            _game.State.Player.Tint = Colors.White;
            TopBar.InventoryButton.Tint = Colors.White;
            _window.Tint = Colors.Black;
            return Task.CompletedTask;
        }

        public async void Show()
        {
            addComboboxToUI(_targetCombobox);
            addComboboxToUI(_tweenCombobox);
            addComboboxToUI(_easeCombobox);
            addComboboxToUI(_repeatCombobox);
            _game.State.UI.Add(_addTweenButton);
            _game.State.UI.Add(_clearTweensButton);
            _game.State.UI.Add(_tweensListScrollingPanel);
            _game.State.UI.Add(_tweensListContentsPanel);
            _game.State.Viewport.Camera.Enabled = false;
            await AGSMessageBox.DisplayAsync("Don't freak out! We're going to change some of the colors now to make the colored tween effects more visible.");
            _game.State.Player.Tint = Color.FromHsla(0, 0.8f, 0.5f, 255);
            TopBar.InventoryButton.Tint = _game.State.Player.Tint;
            _window.Tint = _game.State.Player.Tint.WithAlpha(200);
        }

        private async void addTween()
        {
            LoopingStyle looping = (LoopingStyle)Enum.Parse(typeof(LoopingStyle),getSelection(_repeatCombobox));
            var targetText = getSelection(_targetCombobox);
            var tweenText = getSelection(_tweenCombobox);
            var targetTweens = _tweens[targetText];
            var tween = targetTweens.First(t => t.propertyName == tweenText).getTween();
            if (tween == null)
            {
                await AGSMessageBox.DisplayAsync("Can't play the tween (perhaps there is no music playing currently?)");
                return;
            }
            tween = tween.RepeatForever(looping, 3f);
            _runningTweens.Add(new RunningTween($"{targetText}.{tweenText}", 
                                                tween, _game, _tweensListContentsPanel));
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
                                .Select(k => new AGSStringItem { Text = k.propertyName}).Cast<IStringItem>().ToList());
            if (!_tweenCombobox.DropDownButton.Enabled)
            {
                _tweenCombobox.DropDownButton.Enabled = true;
                _tweenCombobox.TextBox.FadeOut(0.25f);
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

        private async void onItemSelected(ListboxItemArgs args)
        {
            if (hasSelection(_tweenCombobox) && hasSelection(_targetCombobox) &&
                hasSelection(_easeCombobox) && hasSelection(_repeatCombobox) && !_addTweenButton.Enabled)
            {
                await _addTweenButton.FadeOut(0.25f).Task;
                _addTweenButton.Enabled = true;
            }
        }

        private IButton addButton(string id, string text, float x, float y, Action onClick)
        {
            ITextConfig idleConfig = _game.Factory.Fonts.GetTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.White));
            ITextConfig hoverConfig = _game.Factory.Fonts.GetTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.Black));
            var borders = _game.Factory.Graphics.Borders;
            var idle = new ButtonAnimation(borders.SolidColor(Colors.AliceBlue, 5f), idleConfig, Colors.Transparent);
            var hovered = new ButtonAnimation(borders.SolidColor(Colors.Goldenrod, 5f), hoverConfig, Colors.Yellow);
            var pushed = new ButtonAnimation(borders.SolidColor(Colors.AliceBlue, 7f), idleConfig, Colors.Transparent);
            var button = _game.Factory.UI.GetButton(id, idle, hovered, pushed, x, y, _parent, text, null, false, 150f, 50f);
            button.MouseClicked.Subscribe(m => onClick());
            return button;
        }

        private IComboBox addCombobox(string id, float x, float y, string initialText, Action<ListboxItemArgs> callback, params string[] options)
        {
            var combo = _game.Factory.UI.GetComboBox(id, null, null, null, _parent, false, 220f, watermark: initialText);
            combo.Position = (x, y);
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
            _game.State.UI.Add(combo.TextBox.Watermark);
            _game.State.UI.Add(combo);
        }

        private void removeComboboxFromUI(IComboBox combo)
        {
            _game.State.UI.Remove(combo.TextBox);
            _game.State.UI.Remove(combo.DropDownButton);
            _game.State.UI.Remove(combo.TextBox.Watermark);
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
                var borders = game.Factory.Graphics.Borders;
                var tweenId = ++id;
                _panel = factory.GetPanel($"FeaturesTweenPanel_{tweenId}", 300f, 20f, 0f, 0f, null, false);
                _panel.Visible = false;
                _panel.RenderLayer = parent.RenderLayer;
                _panel.Tint = Colors.Transparent;
                _panel.Pivot = (0f, 1f);

                _slider = factory.GetSlider($"FeaturesTweenSlider_{tweenId}", null, null, 0f, 0f, tween.DurationInTicks, _panel);
                _slider.Position = (10f, 10f);
                _slider.HandleGraphics.Pivot = (0.5f, 0.5f);
                _slider.Direction = SliderDirection.LeftToRight;
                _slider.Graphics.Pivot = (0f, 0.5f);
                _slider.Graphics.Image = new EmptyImage(_panel.Width - 100f, 10f);
                _slider.Graphics.Border = borders.SolidColor(Colors.DarkGray, 0.5f, true);
                _slider.HandleGraphics.Border = borders.SolidColor(Colors.White, 0.5f, true);
                HoverEffect.Add(_slider.Graphics, Colors.Green, Colors.LightGray);
                HoverEffect.Add(_slider.HandleGraphics, Colors.DarkGreen, Colors.WhiteSmoke);

                _label = factory.GetLabel($"FeaturesTweenLabel_{tweenId}", name, 100f, 20f,
                                          _slider.X + _slider.Graphics.Width / 2f, _slider.Y + 10f, _panel,
                                          _game.Factory.Fonts.GetTextConfig(autoFit: AutoFit.TextShouldFitLabel));
                _label.Pivot = (0.5f, 0f);

                ITextConfig idleConfig = _game.Factory.Fonts.GetTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.White));
                ITextConfig hoverConfig = _game.Factory.Fonts.GetTextConfig(alignment: Alignment.MiddleCenter, brush: _game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.Black));

                var idle = new ButtonAnimation(borders.SolidColor(Colors.AliceBlue, 2f), idleConfig, Colors.Transparent);
                var hovered = new ButtonAnimation(borders.SolidColor(Colors.Goldenrod, 2f), hoverConfig, Colors.Yellow);
                var pushed = new ButtonAnimation(borders.SolidColor(Colors.AliceBlue, 4f), idleConfig, Colors.Transparent);

                _rewindButton = factory.GetButton($"FeaturesTweenRewindButton_{tweenId}", idle, hovered, pushed, 235f, 0f, _panel, "Rewind", width: 100f, height: 30f);
                _playPauseButton = factory.GetButton($"FeaturesTweenPlayPauseButton_{tweenId}", idle, hovered, pushed, 345f, 0f, _panel, "Pause", width: 100f, height: 30f);
                _stopButton = factory.GetButton($"FeaturesTweenStopButton_{tweenId}", idle, hovered, pushed, 455f, 0f, _panel, "Stop", width: 100f, height: 30f);
                _stopButton.MouseClicked.Subscribe(_ => Stop());
                _rewindButton.MouseClicked.Subscribe(_ => _tween.Rewind());
                _playPauseButton.MouseClicked.Subscribe(onPlayPauseClick);
                _slider.OnValueChanging.Subscribe(onSliderValueChanging);

                _panel.TreeNode.SetParent(parent.TreeNode);
                game.State.UI.Add(_panel);
                _panel.Visible = true;

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
