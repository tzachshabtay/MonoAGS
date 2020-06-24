using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NVorbis;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class OggDecoder : ISoundDecoder
	{
		private const int DEFAULT_BUFFER_SIZE = 1024 * 16;
		private static object _readMutex = new object();

		#region ISoundDecoder implementation

		public ISoundData Decode(Stream stream)
		{
			VorbisReader reader = null;
			try
			{
				reader = new VorbisReader (stream, false);

				int channels = reader.Channels;
				const int BITS_PER_SAMPLE = 16;
				int sampleRate = reader.SampleRate;

				//Code from: https://github.com/AdamsLair/duality/blob/a911c46b6bc830c05d3bb1202f8e7060543eaefa/Duality/Audio/OggVorbis.cs
				List<float[]> allBuffers = new List<float[]>();
				bool eof = false;
				int totalSamplesRead = 0;
				int dataRead = 0;
				while (!eof)
				{
					float[] buffer = new float[DEFAULT_BUFFER_SIZE];
					int bufferSamplesRead = 0;
					while(bufferSamplesRead < buffer.Length)
					{
						int samplesRead;
						lock (_readMutex)
						{
							samplesRead = reader.ReadSamples(buffer, dataRead, buffer.Length - dataRead);
						}
						if (samplesRead > 0)
						{
							bufferSamplesRead += samplesRead;
						}
						else
						{
							eof = true;
							break;
						}
					}
					allBuffers.Add(buffer);
					totalSamplesRead += bufferSamplesRead;
				}

				if (totalSamplesRead > 0)
				{
					short[] data = new short[totalSamplesRead];
					int offset = 0;
					for (int i = 0; i < allBuffers.Count; i++)
					{
						int len = Math.Min(totalSamplesRead - offset, allBuffers[i].Length);
						castBuffer(allBuffers[i], data, offset, len);
						offset += len;
					}
					return new SoundData(channels, BITS_PER_SAMPLE, sampleRate, data, Marshal.SizeOf<short>() * data.Length);
				}
				else
				{
					Debug.WriteLine("OggDecoder: No samples found in ogg file");
					return null;
				}
			}
			catch (InvalidDataException e)
			{
				Debug.WriteLine("OggDecoder: Failed to read ogg. " + e);
				return null;
			}
			finally
			{
				reader?.Dispose();
			}
		}

		#endregion

		private static void castBuffer(float[] source, short[] target, int targetOffset, int length)
		{
			for (int i = 0; i < length; i++)
			{
				var temp = (int)(32767f * source[i]);
				if (temp > short.MaxValue) temp = short.MaxValue;
				else if (temp < short.MinValue) temp = short.MinValue;
				target[targetOffset + i] = (short)temp;
			}
		}

	}
}

