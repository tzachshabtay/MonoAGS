namespace AGS.Engine
{
	public interface ISoundData
	{
		int Channels { get; }
		int BitsPerSample { get; }
		int SampleRate { get; }
		int DataLength { get; }
		object Data { get; }
	}
}    
     
     
