using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class AGSApproachComponent : AGSComponent, IApproachComponent
    {
        private IFaceDirectionComponent _faceDirection;
        private IWalkComponent _walk;

        public AGSApproachComponent()
        {
            ApproachStyle = new AGSApproachStyle();
        }

        public IApproachStyle ApproachStyle { get; set; }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IFaceDirectionComponent>(c => _faceDirection = c, _ => _faceDirection = null);
            Entity.Bind<IWalkComponent>(c => _walk = c, _ => _walk = null);
        }

        public async Task<bool> ApproachAsync(string verb, IObject obj)
        {
            ApproachHotspots approachStyle = getApproachStyle(verb);
            var faceDirection = _faceDirection;
            var walk = _walk;
            var hotspot = obj.GetComponent<IHotspotComponent>();
            var walkPt = hotspot?.WalkPoint;
            switch (approachStyle)
            {
                case ApproachHotspots.NeverWalk:
                    break;
                case ApproachHotspots.FaceOnly:
                    if (faceDirection != null) await faceDirection.FaceDirectionAsync(obj);
                    break;
                case ApproachHotspots.WalkIfHaveWalkPoint:
                    if (walk != null && walkPt != null && !await walk.WalkAsync(new Position(walkPt.Value))) return false;
                    if (faceDirection != null) await faceDirection.FaceDirectionAsync(obj);
                    break;
                case ApproachHotspots.AlwaysWalk:
                    PointF? walkPoint = walkPt ?? obj.CenterPoint ?? obj.Position.XY;
                    if (walk != null && !await walk.WalkAsync(new Position(walkPoint.Value))) return false;
                    if (faceDirection != null) await _faceDirection.FaceDirectionAsync(obj);
                    break;
                default:
                    throw new NotSupportedException("Approach style is not supported: " + approachStyle.ToString());
            }
            return true;
        }

        private ApproachHotspots getApproachStyle(string verb)
        {
            if (!ApproachStyle.ApproachWhenVerb.TryGetValue(verb, out var approachHotspots))
            {
                approachHotspots = ApproachHotspots.NeverWalk;
                ApproachStyle.ApproachWhenVerb[verb] = approachHotspots;
            }
            return approachHotspots;
        }
    }
}
