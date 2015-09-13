using System;
using API;

namespace Engine
{
	public interface IAGSEdges : IEdges
	{
		void OnRepeatedlyExecute(ICharacter character);
	}
}

