using AGS.API;

namespace AGS.Engine
{
    public class AGSTextComponent : AGSComponent, ITextComponent
    {
        private ILabelRenderer _labelRenderer;
        private IImageComponent _obj;
        private IScaleComponent _scale;
        private readonly IGameEvents _events;

        public AGSTextComponent(ILabelRenderer labelRenderer, IGameEvents events)
        {
            _labelRenderer = labelRenderer;
            _events = events;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IImageComponent>(c => { _obj = c; c.CustomRenderer = _labelRenderer; }, _ => _obj = null);
            entity.Bind<IScaleComponent>(c => { _scale = c; }, _ => { _scale = null; });
            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public override void Dispose()
        {
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
        }

        public ITextConfig TextConfig
        {
            get { return _labelRenderer.Config; }
            set { _labelRenderer.Config = value; }
        }

        public string Text
        {
            get { return _labelRenderer.Text; }
            set { _labelRenderer.Text = value; }
        }

        public bool TextVisible
        {
            get { return _labelRenderer.TextVisible; }
            set { _labelRenderer.TextVisible = value; }
        }

        public SizeF LabelRenderSize
        {
            get { return _labelRenderer.BaseSize; }
            set
            {
                _labelRenderer.BaseSize = value;
                var obj = _obj;
                if (obj != null && obj.Image == null) obj.Image = new EmptyImage(value.Width, value.Height);                
            }
        }

        public float TextHeight { get { return _labelRenderer.TextHeight; } }

        public float TextWidth { get { return _labelRenderer.TextWidth; } }

        private void onRepeatedlyExecute(object args)
        {
            var config = TextConfig;
            if (config == null) return;
            if (config.AutoFit != AutoFit.LabelShouldFitText && 
                config.AutoFit != AutoFit.TextShouldWrapAndLabelShouldFitHeight) return;
            var scale = _scale;
            if (scale == null) return;
            scale.ResetBaseSize(_labelRenderer.Width, _labelRenderer.Height);
        }
    }
}
