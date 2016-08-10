using System;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;

namespace AGS.Engine
{
	public class ALErrors : IAudioErrors
	{
		public bool HasErrors()
		{
			var error = AL.GetError();
			if (error == ALError.NoError) return false;
			Debug.WriteLine("OpenAL Error: " + error);
			return true;
		}

	}
}

