using System.Collections.Generic;

namespace AGS.API
{
    public interface IPathFinder
	{
		void Init(bool[][] mask);

		IEnumerable<ILocation> GetWalkPoints(ILocation from, ILocation to);
	}
}

