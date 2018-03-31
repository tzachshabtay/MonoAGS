using System;
using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    public class AGSBorderComponent : AGSComponent, IBorderComponent
    {
        private IBoundingBoxComponent _box;
        private IEntity _entity;
        private readonly IRenderPipeline _pipeline;
        private readonly BorderBack _back;
        private readonly BorderFront _front;

        public AGSBorderComponent(IRenderPipeline pipeline)
        {
            _pipeline = pipeline;
            _back = new BorderBack(this);
            _front = new BorderFront(this);
        }

        public IBorderStyle Border { get; set; }

		public override void Init(IEntity entity)
		{
            base.Init(entity);
            _entity = entity;
            entity.Bind<IBoundingBoxComponent>(c => _box = c, _ => _box = null);
            _pipeline.Subscribe(entity.ID, _back, 100);
            _pipeline.Subscribe(entity.ID, _front, -100);
		}

		public override void Dispose()
		{
            base.Dispose();
            var entity = _entity;
            if (entity == null) return;
            _pipeline.Unsubscribe(entity.ID, _back);
            _pipeline.Unsubscribe(entity.ID, _front);
		}

		private class BorderBack : IRenderer
        {
            private AGSBorderComponent _border;

            public BorderBack(AGSBorderComponent border) => _border = border;

            public IRenderInstruction GetNextInstruction(IViewport viewport)
            {
                var border = _border.Border;
                if (border == null) return null;
                var boxComponent = _border._box;
                if (boxComponent == null) return null;
                var box = boxComponent.GetBoundingBoxes(viewport).ViewportBox;
                if (box.BottomLeft.X > box.BottomRight.X) box = box.FlipHorizontal();
                return new BorderBackInstruction { Border = border, Box = box };
            }
        }

        private class BorderFront : IRenderer
        {
            private AGSBorderComponent _border;

            public BorderFront(AGSBorderComponent border) => _border = border;

            public IRenderInstruction GetNextInstruction(IViewport viewport)
            {
                var border = _border.Border;
                if (border == null) return null;
                var boxComponent = _border._box;
                if (boxComponent == null) return null;
                var box = boxComponent.GetBoundingBoxes(viewport).ViewportBox;
                if (box.BottomLeft.X > box.BottomRight.X) box = box.FlipHorizontal();
                return new BorderFrontInstruction { Border = border, Box = box };
            }
        }

        private class BorderBackInstruction : IRenderInstruction
        {
            public IBorderStyle Border { get; set; }
            public AGSBoundingBox Box { get; set; }

            public void Release()
            {
            }

            public void Render() => Border.RenderBorderBack(Box);
        }

        private class BorderFrontInstruction : IRenderInstruction
        {
            public IBorderStyle Border { get; set; }
            public AGSBoundingBox Box { get; set; }

            public void Release()
            {
            }

            public void Render() => Border.RenderBorderFront(Box);
        }
    }
}