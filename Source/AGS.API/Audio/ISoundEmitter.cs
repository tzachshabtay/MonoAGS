namespace AGS.API
{
	public interface ISoundEmitter : ISoundPlayer
	{
		IAudioClip AudioClip { get; set; }
		
        IObject Object { set; }
        IAnimationContainer AnimationContainer { get; set; }
        IHasRoom HasRoom { get; set; }

		bool AutoPan { get; set; }
		bool AutoAdjustVolume { get; set; }

		void Assign(IDirectionalAnimation animation, params int[] frames);
		void Assign(IAnimation animation, params int[] frames);
		void Assign(params IAnimationFrame[] frames);
	}
}

