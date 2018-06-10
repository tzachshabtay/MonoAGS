using System;
using AGS.API;
using System.Collections.Generic;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSRendererLoop : IRendererLoop
	{
        private readonly IGameState _gameState;
        private readonly IGameSettings _settings;
        private readonly IMatrixUpdater _matrixUpdater;
        private readonly IWindowInfo _window;
        private readonly IAGSRenderPipeline _pipeline;
        private readonly IGLUtils _glUtils;
        private IShader _lastShaderUsed;
		
        public AGSRendererLoop (IGameSettings settings, IGameState state,
            IGLUtils glUtils, IWindowInfo window, IAGSRenderPipeline pipeline,
            IMatrixUpdater matrixUpdater)
		{
            _pipeline = pipeline;
            _glUtils = glUtils;
            _window = window;
            _settings = settings;
			_gameState = state;
            _matrixUpdater = matrixUpdater;
		}

		#region IRendererLoop implementation

        public void Tick()
        {
            _glUtils.RefreshViewport(_settings, _window, _gameState.Viewport, true);
            _glUtils.AdjustResolution(_settings.VirtualResolution.Width, _settings.VirtualResolution.Height);

            activateShader();
            renderAllViewports();
		}

		#endregion

        private void renderAllViewports()
		{
            _matrixUpdater.ClearCache();
            var instructionSet = _pipeline.InstructionSet;
            if (instructionSet == null) return;
            foreach (var (viewport, instructions) in instructionSet)
            {
                renderViewport(viewport, instructions);
            }
		}

        private void renderViewport(IViewport viewport, List<IRenderBatch> instructions)
        {
            _glUtils.RefreshViewport(_settings, _window, viewport, true);

            foreach (var batch in instructions)
            {
                renderBatch(batch);
            }
        }

        private void renderBatch(IRenderBatch batch)
        {
            _glUtils.AdjustResolution(batch.Resolution.Width, batch.Resolution.Height);

            var shader = applyObjectShader(batch.Shader);

            foreach (var instruction in batch.Instructions)
            {
                instruction.Render();
                instruction.Release();
            }

            removeObjectShader(shader);
        }

		private static IShader applyObjectShader(IShader shader)
		{
			if (shader != null) shader = shader.Compile();
			shader?.Bind();
			return shader;
		}

		private void removeObjectShader(IShader shader)
		{
			if (shader == null) return;

			if (_lastShaderUsed != null) _lastShaderUsed.Bind();
			else shader.Unbind();
		}

		private void activateShader()
		{
			var shader = AGSGame.Shader;
			if (shader != null) shader = shader.Compile();
			if (shader == null)
			{
		        _lastShaderUsed?.Unbind();
				return;
			}
			_lastShaderUsed = shader;
			shader.Bind();
		}
	}
}