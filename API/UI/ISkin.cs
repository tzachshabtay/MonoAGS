using System;

namespace API
{
	public interface ISkin
	{
		IButton ButtonSkin { get; }
		ILabel LabelSkin { get; }
	}
}

