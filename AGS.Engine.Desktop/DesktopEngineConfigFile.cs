using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using AGS.Engine;

namespace AGS.Engine.Desktop
{
	public class DesktopEngineConfigFile : IEngineConfigFile
	{
		public bool DebugResolves
		{
			get { return readBool("DebugResolves", false); }
			set { addUpdateAppSettings("DebugResolves", value.ToString()); }
		}

		private void addUpdateAppSettings(string key, string value)
		{
			try
			{
				var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var settings = configFile.AppSettings.Settings;
				if (settings[key] == null)
				{
					settings.Add(key, value);
				}
				else
				{
					settings[key].Value = value;
				}
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
			}
			catch (ConfigurationErrorsException)
			{
				Debug.WriteLine("Error writing app settings");
			}
		}

		private bool readBool(string key, bool defaultValue)
		{
			string val = readSetting(key);
			if (string.IsNullOrEmpty(val)) return defaultValue;
			bool result;
			if (!bool.TryParse(val, out result)) return defaultValue;
			return result;
		}

		private string readSetting(string key)
		{
			try
			{
				var appSettings = ConfigurationManager.AppSettings;
				string result = appSettings[key];
				return result;
			}
			catch (ConfigurationErrorsException e)
			{
				Debug.WriteLine("Error reading app settings: " + e.ToString());
				return null;
			}
		}

	}
}

