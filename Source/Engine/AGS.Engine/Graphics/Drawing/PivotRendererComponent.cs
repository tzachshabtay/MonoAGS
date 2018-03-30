using System;
using AGS.API;

namespace AGS.Engine
{
    public class PivotRendererComponent : AGSComponent, IRenderer
    {
        private readonly IGLUtils _utils;
        private readonly IAGSRenderPipeline _pipeline;
        private IWorldPositionComponent _worldPosition;
        private IEntity _entity;

        public PivotRendererComponent(IGLUtils utils, IAGSRenderPipeline pipeline)
        {
            _utils = utils;
            _pipeline = pipeline;
        }

		public override void Init(IEntity entity)
		{
            base.Init(entity);
            _entity = entity;
            entity.Bind<IWorldPositionComponent>(c => _worldPosition = c, _ => _worldPosition = null);
            _pipeline.Subscribe(entity.ID, this, -200);
		}

		public override void Dispose()
		{
            base.Dispose();
            _pipeline.Unsubscribe(_entity.ID, this);
		}

		public IRenderInstruction GetNextInstruction(IViewport viewport)
        {
            var worldPosition = _worldPosition;
            if (worldPosition == null) return null;
            return new Instruction { XY = worldPosition.WorldXY, Utils = _utils };
        }

        private class Instruction : IRenderInstruction
        {
            public PointF XY { get; set; }
            public IGLUtils Utils { get; set; }

            public void Release()
            {
            }

            public void Render()
            {
                Utils.DrawCross(XY.X, XY.Y, 5f, 5f, 1f, 1f, 1f, 1f);
            }
        }
    }
}