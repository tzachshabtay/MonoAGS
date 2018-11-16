using System;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{ 
    public class AGSCropSelfComponent : AGSComponent, ICropSelfComponent
    {
        private IInObjectTreeComponent _tree;

        public AGSCropSelfComponent()
        {
            OnBeforeCrop = new AGSEvent<BeforeCropEventArgs>();
            CropEnabled = true;
        }

        public bool CropEnabled { get; set; }

        public bool CropText { get; set; }

        public RectangleF CropArea { get; set; }

        public AGSCropInfo LastCrop { get; private set; }

        public bool NeverGuaranteedToFullyCrop { get; set; }

        public IBlockingEvent<BeforeCropEventArgs> OnBeforeCrop { get; private set; }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnBeforeCrop?.Dispose();
            OnBeforeCrop = null;
        }

        public bool IsGuaranteedToFullyCrop()
        {
            return isGuaranteedToFullyCrop().isGuaranteed;
        }

        public AGSCropInfo Crop(ref AGSBoundingBox box, BoundingBoxType boundingBoxType, PointF adjustedScale)
        {
            (var isGuaranteed, var cropFromGuarantee) = isGuaranteedToFullyCrop();
            if (isGuaranteed)
            {
                return LastCrop = new AGSCropInfo(default, null, cropFromGuarantee);
            }
            float scaleX = adjustedScale.X;
            float scaleY = adjustedScale.Y;
            float spriteWidth = box.Width / scaleX;
            float spriteHeight = box.Height / scaleY;
            var args = new BeforeCropEventArgs(box, boundingBoxType);
            (FourCorners<Vector2> cropArea, CropFrom cropFrom) = getCropArea(args, spriteWidth, spriteHeight, out float width, out float height);
            if (!CropEnabled)
            {
                return LastCrop = new AGSCropInfo(box, null, CropFrom.None);
            }
            if (width <= 0f || height <= 0f)
            {
                return LastCrop = new AGSCropInfo(default, null, cropFrom);
            }
            width *= scaleX;
            height *= scaleY;
            if (float.IsNaN(width) || float.IsNaN(height))
            {
                return LastCrop = new AGSCropInfo(default, null, cropFrom);
            }

            float boxWidth = box.Width;
            float boxHeight = box.Height;

            float leftForBottomLeft = box.BottomLeft.X;
            float bottomForBottomLeft = box.BottomLeft.Y;

            float leftForTopLeft = box.TopLeft.X;
            float topForTopLeft = MathUtils.Lerp(0f, box.BottomLeft.Y, boxHeight, box.TopLeft.Y, height);

            float rightForTopRight = MathUtils.Lerp(0f, box.TopLeft.X, boxWidth, box.TopRight.X, width);
            float topForTopRight = MathUtils.Lerp(0f, box.BottomRight.Y, boxHeight, box.TopRight.Y, height);

            float rightForBottomRight = MathUtils.Lerp(0f, box.BottomLeft.X, boxWidth, box.BottomRight.X, width);
            float bottomForBottomRight = box.BottomRight.Y;

            var cropX = Math.Max(0f, CropArea.X);
            var cropY = Math.Max(0f, CropArea.Y);
            float offsetX = cropX * scaleX;
            float offsetY = cropY * scaleY;
            AGSBoundingBox croppedBox = new AGSBoundingBox(new Vector3(leftForBottomLeft + offsetX, bottomForBottomLeft + offsetY, box.BottomLeft.Z),
                                                           new Vector3(rightForBottomRight + offsetX, bottomForBottomRight + offsetY, box.BottomRight.Z),
                                                           new Vector3(leftForTopLeft + offsetX, topForTopLeft + offsetY, box.TopLeft.Z),
                                                           new Vector3(rightForTopRight + offsetX, topForTopRight + offsetY, box.TopRight.Z));

            return LastCrop = new AGSCropInfo(croppedBox, cropArea, cropFrom);
        }

        private (bool isGuaranteed, CropFrom cropFrom) isGuaranteedToFullyCrop()
        {
            if (NeverGuaranteedToFullyCrop)
            {
                return (false, CropFrom.None);
            }

            var tree = _tree;
            if (tree == null)
            {
                return (false, CropFrom.None);
            }

            var layout = tree.TreeNode.Parent?.GetComponent<IStackLayoutComponent>();
            if (layout == null)
            {
                return (false, CropFrom.None);
            }

            var previousSibling = tree.TreeNode.FindPreviousSibling(item => item.Visible);
            if (previousSibling == null)
            {
                return (false, CropFrom.None);
            }

            var previousCrop = CropText ? previousSibling.GetComponent<ITextComponent>()?.CustomTextCrop : previousSibling.GetComponent<ICropSelfComponent>();
            if (previousCrop == null)
            {
                return (false, CropFrom.None);
            }

            switch (previousCrop.LastCrop.CropFrom)
            {
                case CropFrom.Left:
                    if (layout.Direction == LayoutDirection.Vertical)
                        return (false, CropFrom.None);
                    return returnIsGuaranteed(layout.AbsoluteSpacing >= 0f && layout.RelativeSpacing >= 0f, previousCrop.LastCrop);
                case CropFrom.Right:
                    if (layout.Direction == LayoutDirection.Vertical)
                        return (false, CropFrom.None);
                    return returnIsGuaranteed(layout.AbsoluteSpacing <= 0f && layout.RelativeSpacing <= 0f, previousCrop.LastCrop);
                case CropFrom.Bottom:
                    if (layout.Direction == LayoutDirection.Horizontal)
                        return (false, CropFrom.None);
                    return returnIsGuaranteed(layout.AbsoluteSpacing >= 0f && layout.RelativeSpacing >= 0f, previousCrop.LastCrop);
                case CropFrom.Top:
                    if (layout.Direction == LayoutDirection.Horizontal)
                        return (false, CropFrom.None);
                    return returnIsGuaranteed(layout.AbsoluteSpacing <= 0f && layout.RelativeSpacing <= 0f, previousCrop.LastCrop);
                default:
                    return (false, CropFrom.None);
            }
        }

        private (bool isGuaranteed, CropFrom cropFrom) returnIsGuaranteed(bool isGuaranteed, AGSCropInfo cropInfo)
        {
            if (isGuaranteed) LastCrop = cropInfo;
            return (isGuaranteed, cropInfo.CropFrom);
        }

        private (FourCorners<Vector2>, CropFrom) getCropArea(BeforeCropEventArgs eventArgs, float spriteWidth, float spriteHeight, out float width, out float height)
        {
            width = spriteWidth;
            height = spriteHeight;
            OnBeforeCrop?.Invoke(eventArgs);
            if (!CropEnabled)
                return (null, CropFrom.None);
            float cropX = CropArea.X;
            float cropY = CropArea.Y;
            height = Math.Min(height, CropArea.Height);
            if (height <= 0f || float.IsNaN(height))
                return (null, cropY < 0f ? CropFrom.Bottom : CropFrom.Top);
            width = Math.Min(width, CropArea.Width);
            if (width <= 0f || float.IsNaN(width))
                return (null, cropX < 0f ? CropFrom.Left : CropFrom.Right);
            if (cropX < 0f) cropX = 0f;
            if (cropX + width > spriteWidth)
                width = spriteWidth - cropX;
            if (cropY < 0f) cropY = 0f;
            if (cropY + height > spriteHeight)
                height = spriteHeight - cropY;
            float left = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, cropX);
            float right = MathUtils.Lerp(0f, 0f, spriteWidth, 1f, cropX + width);
            float top = MathUtils.Lerp(0f, 1f, spriteHeight, 0f, cropY + height);
            float bottom = MathUtils.Lerp(0f, 1f, spriteHeight, 0f, cropY);

            var textureBox = new FourCorners<Vector2>(new Vector2(left, bottom), new Vector2(right, bottom),
                new Vector2(left, top), new Vector2(right, top));
            return (textureBox, CropFrom.None);
        }
    }
}