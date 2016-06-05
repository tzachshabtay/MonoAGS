using System;
using System.IO;
using System.Diagnostics;

namespace AGS.Engine
{
	//https://github.com/mono/opentk/blob/master/Source/Examples/OpenAL/1.1/Playback.cs
	//https://github.com/mono/opentk/blob/master/Source/Compatibility/Audio/WaveReader.cs
	public class WaveDecoder : ISoundDecoder
	{
		#region ISoundDecoder implementation

		public ISoundData Decode(Stream stream)
		{
			BinaryReader reader = new BinaryReader (stream); //We're not "using" the reader because we want to keep the stream alive, it's up to the caller to dispose the stream

			// RIFF header
			string signature = new string (reader.ReadChars(4));
			if (signature != "RIFF")
			{
				Debug.WriteLine("Specified stream is not a wave file (not a RIFF header).");
				return null;
			}

			reader.ReadInt32(); //riff_chunk_size

			string format = new string (reader.ReadChars(4));
			if (format != "WAVE")
			{
				Debug.WriteLine("Specified stream is not a wave file (not a WAVE header).");
				return null;
			}

			// WAVE header
			string format_signature = new string (reader.ReadChars(4));
			if (format_signature != "fmt ")
			{
				Debug.WriteLine("Specified wave file is not supported (no fmt signature).");
				return null;
			}

			int fmtSize = reader.ReadInt32(); //format_chunk_size
			int audioFormat = reader.ReadInt16(); //audio_format
			if (audioFormat != 1)
			{
				Debug.WriteLine("Audio format is not supported: " + audioFormat);
				return null;
			}
			int channels = reader.ReadInt16();
			int sampleRate = reader.ReadInt32();
			reader.ReadInt32(); //byte_rate
			reader.ReadInt16(); //block_align
			int bitsPerSample = reader.ReadInt16();
			if (bitsPerSample != 8 && bitsPerSample != 16)
			{
				//To support additional bit depths, apart for the parsing, we also need to change ALSound.getSoundFormat
				Debug.WriteLine("Bit depth not supported: " + bitsPerSample);
				return null;
			}

			if (fmtSize == 18)
			{
				// Read any extra values
				int fmtExtraSize = reader.ReadInt16();
				reader.ReadBytes(fmtExtraSize);
			}

			string data_signature = new string (reader.ReadChars(4));

			if (data_signature == "LIST")
			{
				int listChunkSize = reader.ReadInt32();
				reader.ReadBytes(listChunkSize);
				data_signature = new string (reader.ReadChars(4));
			}

			if (data_signature != "data")
			{
				Debug.WriteLine("Specified wave file is not supported (no data signature).");
				return null;
			}

			int dataChunkSize = reader.ReadInt32(); 
			byte[] contents = reader.ReadBytes(dataChunkSize);
			//byte[] contents = reader.ReadBytes((int)reader.BaseStream.Length);
		
			return new SoundData (channels, bitsPerSample, sampleRate, contents, contents.Length);
		}
			
		#endregion
	}
}

