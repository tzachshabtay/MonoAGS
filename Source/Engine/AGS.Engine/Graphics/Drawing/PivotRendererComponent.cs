using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    [RequiredComponent(typeof(IImageComponent))]
    public class PivotRendererComponent : AGSComponent, IRenderer
    {
        private readonly IAGSRenderPipeline _pipeline;
        private readonly ObjectPool<Instruction> _pool;
        private IBoundingBoxComponent _boundingBox;
        private IImageComponent _image;

        public PivotRendererComponent(IGLUtils utils, IAGSRenderPipeline pipeline)
        {
            _pipeline = pipeline;
            _pool = new ObjectPool<Instruction>(pool => new Instruction(pool, utils), 2);
        }

		public override void Init()
		{
            base.Init();
            Entity.Bind<IBoundingBoxComponent>(c => _boundingBox = c, _ => _boundingBox = null);
            Entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
            _pipeline.Subscribe(Entity.ID, this, -200);
		}

		public override void Dispose()
		{
            var entity = Entity;
            if (entity == null) return;
            _pipeline.Unsubscribe(entity.ID, this);
            base.Dispose();
		}

		public IRenderInstruction GetNextInstruction(IViewport viewport)
        {
            var boxComponent = _boundingBox;
            if (boxComponent == null) return null;
            var image = _image;
            if (image == null) return null;
            var pivot = _image.Pivot;
            var box = boxComponent.GetBoundingBoxes(viewport).ViewportBox;
            var viewX = MathUtils.Lerp(0f, box.MinX, 1f, box.MaxX, pivot.X);
            var viewY = MathUtils.Lerp(0f, box.MinY, 1f, box.MaxY, pivot.Y);
            var instruction = _pool.Acquire();
            if (instruction == null) return null;
            instruction.Setup(viewX, viewY);
            return instruction;
        }

        private class Instruction : IRenderInstruction
        {
            private readonly ObjectPool<Instruction> _pool;
            private readonly IGLUtils _utils;
            private float _x, _y;

            public Instruction(ObjectPool<Instruction> pool, IGLUtils utils)
            {
                _pool = pool;
                _utils = utils;
            }

            public void Setup(float x, float y)
            {
                _x = x;
                _y = y;
            }

            public void Release()
            {
                _pool.Release(this);
            }

            public void Render()
            {
                _utils.DrawCross(_x, _y, 5f, 5f, 1f, 1f, 1f, 1f);
            }
        }
    }
}
