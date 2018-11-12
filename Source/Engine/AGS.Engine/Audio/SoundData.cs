namespace AGS.Engine
{
	public class SoundData : ISoundData
	{
		public SoundData(int channels, int bitsPerSample, int sampleRate, object data, int dataLength)
		{
			Channels = channels;
			BitsPerSample = bitsPerSample;
			SampleRate = sampleRate;
			Data = data;
			DataLength = dataLength;
		}

		public int Channels { get; private set; }
		public int BitsPerSample { get; private set; }
		public int SampleRate { get; private set; }
		public int DataLength { get; private set; }
		public object Data { get; private set; }
	}
}

