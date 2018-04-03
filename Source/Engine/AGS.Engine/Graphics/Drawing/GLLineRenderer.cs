using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IDrawableInfoComponent), false)]
    public class GLLineRenderer : AGSComponent, IRenderer
	{
        private readonly IGLUtils _glUtils;
        private IDrawableInfoComponent _drawable;
        private IRenderPipeline _pipeline;
        private IEntity _entity;

        public GLLineRenderer (IGLUtils glUtils, IRenderPipeline pipeline)
		{
            _pipeline = pipeline;
            _glUtils = glUtils;
		}

		public override void Init(IEntity entity)
		{
            base.Init(entity);
            _entity = entity;
            entity.Bind<IDrawableInfoComponent>(c => _drawable = c, _ => _drawable = null);
            _pipeline.Subscribe(entity.ID, this);
		}

		public override void Dispose()
		{
            base.Dispose();
            var entity = _entity;
            if (entity == null) return;
            _pipeline.Unsubscribe(entity.ID, this);
		}

		public void Render (IObject obj, IViewport viewport)
		{
			float x1 = obj.IgnoreViewport ? X1 : X1 - viewport.X;
			float x2 = obj.IgnoreViewport ? X2 : X2 - viewport.X;
			_glUtils.DrawLine (x1, Y1, x2, Y2, 1f, 1f, 0f, 0f, 1f);
		}

		public IRenderInstruction GetNextInstruction(IViewport viewport)
        {
            bool ignoreViewport = _drawable?.IgnoreViewport ?? false;
            float x1 = ignoreViewport ? X1 : X1 - viewport.X;
            float x2 = ignoreViewport ? X2 : X2 - viewport.X;
            return new Instruction { Utils = _glUtils, X1 = x1, X2 = x2, Y1 = Y1, Y2 = Y2 };
        }

        public float X1 { get; set; }
		public float Y1 { get; set; }
		public float X2 { get; set; }
		public float Y2 { get; set; }

        private class Instruction : IRenderInstruction
        {
            public float X1 { get; set; }
            public float Y1 { get; set; }
            public float X2 { get; set; }
            public float Y2 { get; set; }
            public IGLUtils Utils { get; set; }

            public void Release()
            {
            }

            public void Render()
            {
                Utils.DrawLine(X1, Y1, X2, Y2, 1f, 1f, 0f, 0f, 1f);
            }
        }
    }
}