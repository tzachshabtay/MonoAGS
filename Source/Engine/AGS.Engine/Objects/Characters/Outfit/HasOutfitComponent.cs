using AGS.API;

namespace AGS.Engine
{
	public class HasOutfitComponent : AGSComponent, IOutfitComponent
	{
        private IOutfit _outfit;
        private IFaceDirectionComponent _faceDirection;
        private IAnimationComponent _animComp;

		public IOutfit Outfit
        {
            get => _outfit;
            set
            {
                _outfit = value;
                switchToIdle(value);
            }
        }

        public bool SwitchToIdleWhenSwitchingOutfit { get; set; } = true;

        public override void Init()
        {
            base.Init();
            Entity.Bind<IFaceDirectionComponent>(c => _faceDirection = c, _ => _faceDirection = null);
            Entity.Bind<IAnimationComponent>(c => _animComp = c, _ => _animComp = null);
        }

        private void switchToIdle(IOutfit toOutfit)
        {
            if (!SwitchToIdleWhenSwitchingOutfit || toOutfit == null) return;

            var idle = toOutfit[AGSOutfit.Idle];
            if (idle == null) return;

            var animComp = _animComp;
            if (animComp == null) return;

            var faceDirection = _faceDirection;
            var direction = faceDirection?.Direction ?? Direction.Down;

            var animation = idle.GetAnimation(direction) ?? idle.GetAnimation(Direction.Down);
            animComp.StartAnimation(animation);
        }
	}
}