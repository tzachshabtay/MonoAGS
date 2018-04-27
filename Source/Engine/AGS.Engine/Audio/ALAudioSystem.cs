using System;

namespace AGS.Engine
{
	public class ALAudioSystem : IDisposable, IALAudioSystem
	{
        private readonly IAudioBackend _backend;
		private readonly IAudioErrors _errors;

        public ALAudioSystem(IAudioBackend backend, IAudioListener listener, IAudioErrors errors)
		{
            _backend = backend;
            Listener = listener;
			_errors = errors;
		}

		#region IAudioSystem implementation

		public IAudioListener Listener { get; private set; }

        public int AcquireSource()
		{
			int source = _backend.GenSource();
			_errors.HasErrors();
			return source;
		}

		public void ReleaseSource(int source)
		{
			_backend.SourceStop(source);
			_backend.DeleteSource(source);
			_errors.HasErrors();
		}

		#endregion

		#region IDisposable implementation
		public void Dispose()
		{
            _backend.Dispose();
		}
		#endregion
	}
}
