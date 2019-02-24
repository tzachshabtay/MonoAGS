using AGS.API;

namespace AGS.Engine
{
    public class AGSCoordinates : ICoordinates
    {
        private readonly IGameState _state;
        private readonly Size _virtualResolution;

        private IViewport _mainViewport => _state.Viewport;

        public AGSCoordinates(IGameState state, IGameSettings settings, IWindowInfo window)
        {
            _state = state;
            _virtualResolution = settings.VirtualResolution;
            Window = window;
        }

        public IWindowInfo Window { get; set; }

        public float ViewportXToWindowX(IViewport viewport, float x)
        {
            var projectLeft = viewport.ScreenArea.X;
            var projectRight = projectLeft + viewport.ScreenArea.Width;

            float windowX = MathUtils.Lerp(0f, projectLeft, _virtualResolution.Width, projectRight, x);
            return windowX;
        }

        public float WindowXToViewportX(IViewport viewport, float x)
        {
            var projectLeft = viewport.ScreenArea.X;
            var projectRight = projectLeft + viewport.ScreenArea.Width;

            float viewX = MathUtils.Lerp(projectLeft, 0f, projectRight, _virtualResolution.Width, x);
            return viewX;
        }

        public float ViewportYToWindowY(IViewport viewport, float y)
        {
            var projectBottom = Window.AppWindowHeight - viewport.ScreenArea.Y - viewport.ProjectionBox.Y * viewport.ScreenArea.Height;
            var projectTop = projectBottom - viewport.ProjectionBox.Height * viewport.ScreenArea.Height;

            var parentBoundingBoxes = viewport.Parent?.GetBoundingBoxes(_mainViewport);
            if (parentBoundingBoxes != null)
            {
                projectBottom -= parentBoundingBoxes.ViewportBox.MinY;
                projectTop -= parentBoundingBoxes.ViewportBox.MinY;
            }

            float windowY = MathUtils.Lerp(0f, projectBottom, _virtualResolution.Height, projectTop, y);
            return windowY;
        }

        public float WindowYToViewportY(IViewport viewport, float y)
        {
            var projectBottom = Window.AppWindowHeight - viewport.ScreenArea.Y - viewport.ProjectionBox.Y * viewport.ScreenArea.Height;
            var projectTop = projectBottom - viewport.ProjectionBox.Height * viewport.ScreenArea.Height;

            var parentBoundingBoxes = viewport.Parent?.GetBoundingBoxes(_mainViewport);
            if (parentBoundingBoxes != null)
            {
                projectBottom -= parentBoundingBoxes.ViewportBox.MinY;
                projectTop -= parentBoundingBoxes.ViewportBox.MinY;
            }

            float viewY = MathUtils.Lerp(projectBottom, 0f, projectTop, _virtualResolution.Height, y);
            return viewY;
        }

        public PointF ViewportToWindow(IViewport viewport, PointF position)
        {
            return (ViewportXToWindowX(viewport, position.X), ViewportYToWindowY(viewport, position.Y));
        }

        public PointF WindowToViewport(IViewport viewport, PointF position)
        {
            return (WindowXToViewportX(viewport, position.X), WindowYToViewportY(viewport, position.Y));
        }

        public PointF WorldToWindow(PointF position) => ViewportToWindow(_mainViewport, position);

        public PointF WindowToWorld(PointF position) => WindowToViewport(_mainViewport, position);

        public float WorldXToWindowX(float x) => ViewportXToWindowX(_mainViewport, x);

        public float WorldYToWindowY(float y) => ViewportYToWindowY(_mainViewport, y);

        public float WindowXToWorldX(float x) => WindowXToViewportX(_mainViewport, x);

        public float WindowYToWorldY(float y) => WindowYToViewportY(_mainViewport, y);

        public PointF WindowToObject(IViewport viewport, IObject obj, PointF position)
        {
            var projectBottom = Window.AppWindowHeight - viewport.ScreenArea.Y - viewport.ProjectionBox.Y * viewport.ScreenArea.Height;
            var projectTop = projectBottom - viewport.ProjectionBox.Height * viewport.ScreenArea.Height;
            var parentBoundingBoxes = viewport.Parent?.GetBoundingBoxes(_mainViewport);
            if (parentBoundingBoxes != null)
            {
                projectBottom -= parentBoundingBoxes.ViewportBox.MinY;
                projectTop -= parentBoundingBoxes.ViewportBox.MinY;
            }

            var projectLeft = viewport.ScreenArea.X;
            var projectRight = projectLeft + viewport.ScreenArea.Width;
            float y = MathUtils.Lerp(projectTop, _virtualResolution.Height, projectBottom, 0f, position.Y);
            float x = MathUtils.Lerp(projectLeft, 0f, projectRight, _virtualResolution.Width, position.X);

            var matrix = viewport.GetMatrix(obj.RenderLayer);
            matrix.Invert();
            Vector3 xyz = new Vector3(x, y, 0f);
            Vector3.Transform(ref xyz, ref matrix, out xyz);
            x = xyz.X;
            y = xyz.Y;

            var hitTestBox = obj.TreeNode.Parent?.WorldBoundingBox;
            if (hitTestBox?.IsValid ?? false)
            {
                x -= hitTestBox.Value.MinX;
                y -= hitTestBox.Value.MinY;
            }
            var renderLayer = obj.RenderLayer;
            if (renderLayer?.IndependentResolution != null)
            {
                float maxX = renderLayer.IndependentResolution.Value.Width;
                float maxY = renderLayer.IndependentResolution.Value.Height;
                x = MathUtils.Lerp(0f, 0f, _virtualResolution.Width, maxX, x);
                y = MathUtils.Lerp(0f, 0f, _virtualResolution.Height, maxY, y);
            }
            return new PointF(x, y);
        }

        public bool IsWorldInWindow(PointF worldPosition) => 
            worldPosition.X >= 0f && worldPosition.X <= _virtualResolution.Width && 
            worldPosition.Y >= 0 && worldPosition.Y <= _virtualResolution.Height;
    }
}