using System;
using AGS.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace AGS.Engine
{
    public class AGSAreaComponent : AGSComponent, IAreaComponent
    {
        private static List<(int stepX,int stepY)> _searchVectors;
        private ITranslateComponent _translate;
        private IRotateComponent _rotate;

        static AGSAreaComponent()
        {
            _searchVectors = new List<(int,int)>
            {
                (0, -1),
                (0, 1),
                (1, 0),
                (-1, 0),
                (-1, -1),
                (1, 1),
            };
        }

        public AGSAreaComponent ()
        {
            Enabled = true;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ITranslateComponent>(
                c => { _translate = c; c.PropertyChanged += onLocationChanged; },
                c => { _translate = null; c.PropertyChanged -= onLocationChanged; });
            entity.Bind<IRotateComponent>(
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
            PointF? result = findClosestPoint(x, y, width, height, out insideDistance);
            distance += insideDistance;
            return result;
        }

        public IMask Mask { get; set; }
        public bool Enabled { get; set; }

        #endregion

        private void onLocationChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ITranslateComponent.Location)) return;
            var translate = _translate;
            var obj = Mask.DebugDraw;
            if (translate == null || obj == null) return;
            obj.Location = translate.Location;
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

        private PointF? findClosestPoint(int x, int y, int width, int height, out float distance)
        {
            //todo: This will not always give the real closest position.
            //It's "good enough" most of the time, but can be improved (it only searches using straight lines currently).
            distance = float.MaxValue;
            PointF? closestPoint = null;
            foreach (var (stepX, stepY) in _searchVectors) 
            {
                float tmpDistance;
                PointF? point = findClosestPoint (x, y, width, height, stepX, stepY, out tmpDistance);
                if (tmpDistance < distance) 
                {
                    closestPoint = point;
                    distance = tmpDistance;
                }
            }
            return closestPoint;
        }
            
        private PointF? findClosestPoint(int x, int y, int width, int height, int stepX, int stepY,
            out float distance)
        {
            distance = 0f;
            while (!Mask.IsMasked(new PointF(x, y)))
            {
                x += stepX;
                y += stepY;
                distance++;
                if (x < 0 || x >= width || y < 0 || y >= height) 
                {
                    distance = float.MaxValue;
                    return null;
                }
            }
            return new PointF (x, y);
        }
    }
}

