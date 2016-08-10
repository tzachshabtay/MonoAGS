using System;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;
using Autofac;

namespace AGS.Engine
{
	public class ALAudioSystem : IDisposable, IAudioSystem
	{
		private AudioContext _context;
		private IAudioErrors _errors;

		public ALAudioSystem(Resolver resolver, IAudioErrors errors)
		{
			_context = new AudioContext ();
			Listener = resolver.Container.Resolve<IAudioListener>();
			_errors = errors;
		}

		#region IAudioSystem implementation

		public IAudioListener Listener { get; private set; }

		public int AcquireSource()
		{
			int source = AL.GenSource();
			_errors.HasErrors();
			return source;
		}

		public void ReleaseSource(int source)
		{
			AL.SourceStop(source);
			AL.DeleteSource(source);
			_errors.HasErrors();
		}
			
		#endregion
			
		#region IDisposable implementation
		public void Dispose()
		{
			_context.Dispose();
		}
		#endregion
	}
}

