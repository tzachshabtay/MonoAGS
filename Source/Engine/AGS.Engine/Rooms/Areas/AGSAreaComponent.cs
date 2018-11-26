using System;
using AGS.API;
using System.ComponentModel;

namespace AGS.Engine
{
    public class AGSAreaComponent : AGSComponent, IAreaComponent
    {
        private ITranslateComponent _translate;
        private IRotateComponent _rotate;

        public AGSAreaComponent ()
        {
            Enabled = true;
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<ITranslateComponent>(
                c => { _translate = c; c.PropertyChanged += onLocationChanged; },
                c => { _translate = null; c.PropertyChanged -= onLocationChanged; });
            Entity.Bind<IRotateComponent>(
                c => { _rotate = c; c.PropertyChanged += onAngleChanged; },
                c => { _rotate = null; c.PropertyChanged -= onAngleChanged; });
        }

        #region IArea implementation

        public bool IsInArea (PointF point)
        {
            return Mask.IsMasked(point);
        }

        public bool IsInArea(PointF point, AGSBoundingBox projectionBox, float scaleX, float scaleY)
        {
            return Mask.IsMasked(point, projectionBox, scaleX, scaleY);
        }

        public PointF? FindClosestPoint (PointF point, out float distance)
        {
            int x = (int)point.X;
            int y = (int)point.Y;
            int width = Mask.Width;
            int height = Mask.Height;
            distance = 0f;
            if (x < 0) 
            {
                distance -= x;
                x = 0;
            }
            if (x >= width) 
            {
                distance += (width - x);
                x = width - 1;
            }
            if (y < 0) 
            {
                distance -= y;
                y = 0;
            }

            if (y >= height) 
            {
                distance += (height - y);
                y = height - 1;
            }
            float insideDistance;
            PointF? result = findClosestPoint(x, y, out insideDistance);
            distance += insideDistance;
            return result;
        }

        public IMask Mask { get; set; }
        public bool Enabled { get; set; }

        #endregion

        private void onLocationChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ITranslateComponent.Position)) return;
            var translate = _translate;
            var obj = Mask.DebugDraw;
            if (translate == null || obj == null) return;
            obj.Position = translate.Position;
            Mask.Transform(createMatrix(_translate, _rotate));
        }

        private void onAngleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IRotateComponent.Angle)) return;
            var rotate = _rotate;
            var obj = Mask.DebugDraw;
            if (rotate == null || obj == null) return;
            obj.Angle = rotate.Angle;
            Mask.Transform(createMatrix(_translate, _rotate));
        }

        private Matrix4 createMatrix(ITranslate translate, IRotate rotate)
        {
            if (translate == null && rotate == null) return Matrix4.Identity;

            var radians = rotate == null ? 0f : MathUtils.DegreesToRadians(rotate.Angle);
            Matrix4 rotation;
            Matrix4.CreateRotationZ(radians, out rotation);
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(translate == null ? 0f : translate.X,
                                                                      translate == null ? 0f : translate.Y, 0f));
            Matrix4 transformation = rotation * translation;
            return transformation;
        }

        private PointF? findClosestPoint(int x, int y, out float distance)
        {
            //todo: This will not always give the real closest position.
            //It's "good enough" most of the time, taken from "classic" AGS code, first we scan every 2 pixels in a close vicinity, if didn't find then looking at every 5 pixels in the entire mask.
            return findClosestPointInRange(x, y, 20, 2, out distance) ?? findClosestPointInRange(x, y, Mask.Width, 5, out distance);
        }
            
        private PointF? findClosestPointInRange(int x, int y, int range, int step, out float distance)
        {
            int startX = Math.Max(0, x - range);
            int startY = Math.Max(0, y - range);
            int endX = Math.Min(Mask.Width, startX + range);
            int endY = Math.Min(Mask.Height, startY + range);
            distance = float.MaxValue;
            PointF? closestPoint = null;
            for (int currentX = startX; currentX < endX; currentX += step)
            {
                for (int currentY = startY; currentY < endY; currentY += step)
                {
                    var currentPoint = new PointF(currentX, currentY);
                    if (!Mask.IsMasked(currentPoint))
                    {
                        continue;
                    }
                    float currentDistance = (currentX - x) * (currentX - x) + (currentY - y) * (currentY - y);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestPoint = currentPoint;
                    }
                }
            }
            distance = (float)Math.Sqrt(distance);
            return closestPoint;
        }
    }
}
