using System.Diagnostics;

namespace AGS.Engine
{
	public class ALErrors : IAudioErrors
	{
        private IAudioBackend _backend;

        public ALErrors(IAudioBackend backend)
        {
            _backend = backend;
        }

		public bool HasErrors()
		{
            var error = _backend.GetError();
			if (error == null) return false;
			Debug.WriteLine("OpenAL Error: " + error);
			return true;
		}

	}
}

