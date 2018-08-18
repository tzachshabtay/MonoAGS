using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSRenderPipeline : IRenderPipeline, IAGSRenderPipeline
    {
        private readonly Dictionary<string, List<(int z, IRenderer)>> _renderers;
        private readonly IGameState _state;
        private readonly IDisplayList _displayList;
        private readonly DisplayListEventArgs _displayListEventArgs;
        private readonly IGame _game;
        private readonly RendererComparer _comparer = new RendererComparer();
        private int _lastCapacity = 200;

        public AGSRenderPipeline(IGameState state, IDisplayList displayList, IGame game,
                                 IBlockingEvent<DisplayListEventArgs> onBeforeProcessingDisplayList)
        {
            _state = state;
            _game = game;
            _displayList = displayList;
            _displayListEventArgs = new DisplayListEventArgs(null);
            OnBeforeProcessingDisplayList = onBeforeProcessingDisplayList;
            _renderers = new Dictionary<string, List<(int z, IRenderer)>>(100);
        }

        public IBlockingEvent<DisplayListEventArgs> OnBeforeProcessingDisplayList { get; private set; }

        public IReadOnlyList<(IViewport, List<IRenderBatch>)> InstructionSet { get; private set; }

        public void Subscribe(string entityID, IRenderer renderer, int z = 0)
        {
            Unsubscribe(entityID, renderer);
            var renderers = _renderers.GetOrAdd(entityID, _ => new List<(int z, IRenderer)>());
            renderers.Add((z, renderer));
            renderers.Sort(_comparer);
        }

        public void Unsubscribe(string entityID, IRenderer renderer)
        {
            _renderers.TryGetValue(entityID, out List<(int, IRenderer other)> renderers);
            if (renderers == null) return;
            for (int index = renderers.Count - 1; index >= 0; index--)
            {
                var other = renderers[index];
                if (renderer == other.other)
                {
                    renderers.RemoveAt(index);
                    if (renderers.Count == 0) _renderers.Remove(entityID);
                    return;
                }
            }
        }

        public void Update()
        {
            if (_game.Settings == null) return;
            var instructions = new List<(IViewport, List<IRenderBatch>)>();

            var viewports = _state.GetSortedViewports();
            try
            {
                for (int i = viewports.Count - 1; i >= 0; i--)
                {
                    var viewport = viewports[i];
                    if (viewport == null) continue;
                    var viewportInstructions = getInstructions(viewport);
                    instructions.Add((viewport, viewportInstructions));
                }
                addCursor(instructions);
                InstructionSet = instructions;
            }
            catch (IndexOutOfRangeException) { } //can be triggered if a viewport was added/removed while enumerating- this should be resolved on next tick
        }

        private void addCursor(List<(IViewport, List<IRenderBatch>)> instructions)
        {
            var cursor = _displayList.GetCursor();
            if (cursor == null) return;
            AGSRenderBatch cursorBatch = new AGSRenderBatch(getResolution(cursor), cursor.Shader, getInstructions(_displayList.GetCursor().ID, _state.Viewport));
            instructions.Add((_state.Viewport, new List<IRenderBatch> { cursorBatch }));
        }

        private Size getResolution(IObject obj)
        {
            Size objResolution = obj.RenderLayer?.IndependentResolution ?? _game.Settings.VirtualResolution;
            return objResolution;
        }

        private List<IRenderBatch> getInstructions(IViewport viewport)
        {
            List<IObject> displayList = _displayList.GetDisplayList(viewport);
            _displayListEventArgs.DisplayList = displayList;
            OnBeforeProcessingDisplayList.Invoke(_displayListEventArgs);
            displayList = _displayListEventArgs.DisplayList;

            List<IRenderBatch> instructions = new List<IRenderBatch>(displayList.Count);

            Size resolution = new Size();
            IShader shader = null;
            List<IRenderInstruction> batch = new List<IRenderInstruction>(_lastCapacity);
            foreach (IObject obj in displayList)
            {
                Size objResolution = getResolution(obj);
                var objShader = obj.Shader;

                if (!resolution.Equals(objResolution) || shader != objShader)
                {
                    if (batch.Count > 0)
                    {
                        var renderBatch = new AGSRenderBatch(resolution, shader, batch);
                        instructions.Add(renderBatch);
                    }
                    resolution = objResolution;
                    shader = obj.Shader;
                    if (batch.Count > _lastCapacity)
                    {
                        _lastCapacity = batch.Count;
                    }
                    batch = new List<IRenderInstruction>(_lastCapacity);
                }
                
                var entityInstructions = getInstructions(obj.ID, viewport);
                batch.AddRange(entityInstructions);
            }

            if (batch.Count > 0)
            {
                var renderBatch = new AGSRenderBatch(resolution, shader, batch);
                instructions.Add(renderBatch);
            }

            return instructions;
        }

        private List<IRenderInstruction> getInstructions(string entityId, IViewport viewport)
        {
            List<IRenderInstruction> instructions = new List<IRenderInstruction>();
            if (_renderers.TryGetValue(entityId, out var renderers))
            {
                foreach (var (_, renderer) in renderers)
                {
                    var instruction = renderer.GetNextInstruction(viewport);
                    if (instruction == null) continue;
                    instructions.Add(instruction);
                }
            }
            return instructions;
        }

        private class RendererComparer : IComparer<(int z, IRenderer)>
        {
            public int Compare((int z, IRenderer) x, (int z, IRenderer) y)
            {
                return y.z - x.z;
            }
        }
    }
}
