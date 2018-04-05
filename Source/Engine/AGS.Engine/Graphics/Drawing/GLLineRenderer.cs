using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IDrawableInfoComponent), false)]
    public class GLLineRenderer : AGSComponent, IRenderer
	{
        private readonly ObjectPool<Instruction> _pool;
        private IDrawableInfoComponent _drawable;
        private readonly IRenderPipeline _pipeline;
        private IEntity _entity;

        public GLLineRenderer (IGLUtils glUtils, IRenderPipeline pipeline)
		{
            _pipeline = pipeline;
            _pool = new ObjectPool<Instruction>(pool => new Instruction(pool, glUtils), 2);
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

		public IRenderInstruction GetNextInstruction(IViewport viewport)
        {
            bool ignoreViewport = _drawable?.IgnoreViewport ?? false;
            float x1 = ignoreViewport ? X1 : X1 - viewport.X;
            float x2 = ignoreViewport ? X2 : X2 - viewport.X;
            var instruction = _pool.Acquire();
            instruction.Setup(x1, Y1, x2, Y2);
            return instruction;
        }

        public float X1 { get; set; }
		public float Y1 { get; set; }
		public float X2 { get; set; }
		public float Y2 { get; set; }

        private class Instruction : IRenderInstruction
        {
            private readonly ObjectPool<Instruction> _pool;
            private readonly IGLUtils _utils;
            private float _x1, _y1, _x2, _y2;

            public Instruction(ObjectPool<Instruction> pool, IGLUtils utils)
            {
                _pool = pool;
                _utils = utils;
            }

            public void Setup(float x1, float y1, float x2, float y2)
            {
                _x1 = x1;
                _y1 = y1;
                _x2 = x2;
                _y2 = y2;
            }

            public void Release()
            {
                _pool.Release(this);
            }

            public void Render()
            {
                _utils.DrawLine(_x1, _y1, _x2, _y2, 1f, 1f, 0f, 0f, 1f);
            }
        }
    }
}