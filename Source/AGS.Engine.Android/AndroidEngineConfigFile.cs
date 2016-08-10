using System;

namespace AGS.Engine.Android
{
	public class AndroidEngineConfigFile : IEngineConfigFile
	{
		public AndroidEngineConfigFile()
		{
		}

		#region IEngineConfigFile implementation

		public bool DebugResolves { get; set; }

		#endregion
	}
}

