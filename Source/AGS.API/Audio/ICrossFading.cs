using System;

namespace AGS.API
{
	public interface ICrossFading
	{
		bool FadeOut { get; set; }
		bool FadeIn { get; set; }

		float FadeOutSeconds { get; set; }
		float FadeInSeconds { get; set; }

		Func<float, float> EaseFadeOut { get; set; }
		Func<float, float> EaseFadeIn { get; set; }
	}
}

