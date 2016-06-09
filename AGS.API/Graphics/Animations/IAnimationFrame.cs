namespace AGS.API
{
    public interface IAnimationFrame
	{
		ISprite Sprite { get; set; }
		ISoundEmitter SoundEmitter { get; set; }

		int Delay { get; set; }
		int MinDelay { get; set; }
		int MaxDelay { get; set; }

		IAnimationFrame Clone();
	}
}

