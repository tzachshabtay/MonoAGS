using System;
using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    public class AGSBorderComponent : AGSComponent, IBorderComponent
    {
        private IBoundingBoxComponent _box;
        private readonly IRenderPipeline _pipeline;
        private BorderBack _back;
        private BorderFront _front;

        public AGSBorderComponent(IRenderPipeline pipeline)
        {
            _pipeline = pipeline;
            var backPool = new ObjectPool<BorderBackInstruction>(pool => new BorderBackInstruction(pool), 2);
            var frontPool = new ObjectPool<BorderFrontInstruction>(pool => new BorderFrontInstruction(pool), 2);
            _back = new BorderBack(this, backPool);
            _front = new BorderFront(this, frontPool);
        }

        public IBorderStyle Border { get; set; }

		public override void Init()
		{
            base.Init();
            Entity.Bind<IBoundingBoxComponent>(c => _box = c, _ => _box = null);
            _pipeline.Subscribe(Entity.ID, _back, 100);
            _pipeline.Subscribe(Entity.ID, _front, -100);
		}

		public override void Dispose()
		{
            var entity = Entity;
            if (entity != null)
            {
                _pipeline.Unsubscribe(entity.ID, _back);
                _pipeline.Unsubscribe(entity.ID, _front);
            }
            _back?.Dispose();
            _front?.Dispose();
            _back = null;
            _front = null;
            base.Dispose();
		}

		private class BorderBack : IRenderer
        {
            private AGSBorderComponent _border;
            private ObjectPool<BorderBackInstruction> _pool;

            public BorderBack(AGSBorderComponent border, ObjectPool<BorderBackInstruction> pool)
            {
                _border = border;
                _pool = pool;
            }

            public IRenderInstruction GetNextInstruction(IViewport viewport)
            {
                var border = _border.Border;
                if (border == null) return null;
                var boxComponent = _border._box;
                if (boxComponent == null) return null;
                var box = boxComponent.GetBoundingBoxes(viewport).ViewportBox;
                if (!box.IsValid) return null;
                if (box.BottomLeft.X > box.BottomRight.X) box = box.FlipHorizontal();
                var instruction = _pool.Acquire();
                if (instruction == null) return null;
                instruction.Setup(border, box);
                return instruction;
            }

            public void Dispose()
            {
                _pool?.Dispose();
            }
        }

        private class BorderFront : IRenderer
        {
            private readonly AGSBorderComponent _border;
            private readonly ObjectPool<BorderFrontInstruction> _pool;

            public BorderFront(AGSBorderComponent border, ObjectPool<BorderFrontInstruction> pool)
            {
                _border = border;
                _pool = pool;
            }

            public IRenderInstruction GetNextInstruction(IViewport viewport)
            {
                var border = _border.Border;
                if (border == null) return null;
                var boxComponent = _border._box;
                if (boxComponent == null) return null;
                var box = boxComponent.GetBoundingBoxes(viewport).ViewportBox;
                if (!box.IsValid) return null;
                if (box.BottomLeft.X > box.BottomRight.X) box = box.FlipHorizontal();
                var instruction = _pool.Acquire();
                if (instruction == null) return null;
                instruction.Setup(border, box);
                return instruction;
            }

            public void Dispose()
            {
                _pool?.Dispose();
            }
        }

        private class BorderBackInstruction : IRenderInstruction
        {
            private readonly ObjectPool<BorderBackInstruction> _pool;
            private IBorderStyle _border;
            private AGSBoundingBox _box;

            public BorderBackInstruction(ObjectPool<BorderBackInstruction> pool)
            {
                _pool = pool;
            }

            public void Setup(IBorderStyle border, AGSBoundingBox box)
            {
                _border = border;
                _box = box;
            }

            public void Release()
            {
                _pool.Release(this);
            }

            public void Render() => _border.RenderBorderBack(_box);
        }

        private class BorderFrontInstruction : IRenderInstruction
        {
            private ObjectPool<BorderFrontInstruction> _pool;
            private IBorderStyle _border;
            private AGSBoundingBox _box;

            public BorderFrontInstruction(ObjectPool<BorderFrontInstruction> pool)
            {
                _pool = pool;
            }

            public void Setup(IBorderStyle border, AGSBoundingBox box)
            {
                _border = border;
                _box = box;
            }

            public void Release()
            {
                _pool.Release(this);
            }

            public void Render() => _border.RenderBorderFront(_box);
        }
    }
}
