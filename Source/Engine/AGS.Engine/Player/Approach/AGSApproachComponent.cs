using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class AGSApproachComponent : AGSComponent, IApproachComponent
    {
        private IFaceDirectionBehavior _faceDirection;
        private IWalkBehavior _walk;

        public AGSApproachComponent()
        {
            ApproachStyle = new AGSApproachStyle();
        }

        public IApproachStyle ApproachStyle { get; set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _faceDirection = entity.GetComponent<IFaceDirectionBehavior>();
            _walk = entity.GetComponent<IWalkBehavior>();
        }

        public async Task<bool> ApproachAsync(string verb, IObject obj)
        {
            ApproachHotspots approachStyle = getApproachStyle(verb);
            switch (approachStyle)
            {
                case ApproachHotspots.NeverWalk:
                    break;
                case ApproachHotspots.FaceOnly:
                    await _faceDirection.FaceDirectionAsync(obj);
                    break;
                case ApproachHotspots.WalkIfHaveWalkPoint:
                    if (obj.WalkPoint == null) await _faceDirection.FaceDirectionAsync(obj);
                    else
                    {
                        if (!await _walk.WalkAsync(new AGSLocation(obj.WalkPoint.Value))) return false;
                        await _faceDirection.FaceDirectionAsync(obj);
                    }
                    break;
                case ApproachHotspots.AlwaysWalk:
                    PointF? walkPoint = obj.WalkPoint ?? obj.CenterPoint ?? obj.Location.XY;
                    if (!await _walk.WalkAsync(new AGSLocation(walkPoint.Value))) return false;
                    await _faceDirection.FaceDirectionAsync(obj);
                    break;
                default:
                    throw new NotSupportedException("Approach style is not supported: " + approachStyle.ToString());
            }
            return true;
        }

        private ApproachHotspots getApproachStyle(string verb)
        {
            ApproachHotspots approachHotspots;
            if (!ApproachStyle.ApproachWhenVerb.TryGetValue(verb, out approachHotspots))
            {
                approachHotspots = ApproachHotspots.NeverWalk;
                ApproachStyle.ApproachWhenVerb[verb] = approachHotspots;
            }
            return approachHotspots;
        }
    }
}
