using System;

namespace API
{
	public interface IEdges
	{
		IEdge Left { get; }
		IEdge Right { get; }
		IEdge Top { get; }
		IEdge Bottom { get; }
	}
}

