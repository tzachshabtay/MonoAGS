using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IDialogLayout
	{
		Task LayoutAsync(IObject dialogGraphics, IList<IDialogOption> options);
	}
}

