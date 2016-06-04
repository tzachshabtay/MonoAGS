using System;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;

namespace AGS.Engine
{
	public class ALAudioSystem : IDisposable, IAudioSystem
	{
		private AudioContext _context;

		public ALAudioSystem()
		{
			_context = new AudioContext ();
		}

		#region IAudioSystem implementation

		public int AcquireSource()
		{
			return AL.GenSource();
		}

		public void ReleaseSource(int source)
		{
			AL.SourceStop(source);
			AL.DeleteSource(source);
		}

		public bool HasErrors()
		{
			var error = AL.GetError();
			if (error == ALError.NoError) return false;
			Debug.WriteLine("OpenAL Error: " + error);
			return true;
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

