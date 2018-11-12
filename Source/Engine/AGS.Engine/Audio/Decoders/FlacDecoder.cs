using System;
using System.IO;
using CSCore.Codecs.FLAC;
using System.Diagnostics;

namespace AGS.Engine
{
	public class FlacDecoder : ISoundDecoder
	{
		#region ISoundDecoder implementation

		public ISoundData Decode(Stream stream)
		{
			try
			{
				FlacFile file = new FlacFile (stream);
				byte[] buffer = new byte[stream.Length];
				file.Read(buffer, 0, buffer.Length);

				return new SoundData (file.WaveFormat.Channels, file.WaveFormat.BitsPerSample, file.WaveFormat.SampleRate,
					buffer, buffer.Length);
			}
			catch (FlacException e)
			{
				Debug.WriteLine("Failed to parse flac file: " + e);
				return null;
			}
		}

		#endregion
	}
}

