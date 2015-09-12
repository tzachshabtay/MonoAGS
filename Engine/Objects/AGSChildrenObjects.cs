using System;
using API;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;

namespace Engine
{
	public class AGSChildrenObjects : IChildrenCollection
	{
		//No ConcurrentList in dot net, and ConcurrentBag doesn't have remove, simplest solution seems to be
		//using a concurrent dictionary
		private ConcurrentDictionary<IObject, IObject> map;
		IObject parent;

		public AGSChildrenObjects(IObject parent)
		{
			this.parent = parent;
			map = new ConcurrentDictionary<IObject, IObject> ();
		}

		#region IChildrenCollection implementation

		public void AddChild(IObject child)
		{
			//Adding a child is a two step process (Parent property for the child changes first)
			if (child.Parent == parent)
				map.TryAdd(child, child);
			else child.Parent = parent;
		}

		public void RemoveChild(IObject child)
		{
			//Removing a child is a two step process (Parent property for the child changes first)
			if (child.Parent != parent)
				map.TryRemove(child, out child);
			else child.Parent = null;
		}

		public bool HasChild(IObject child)
		{
			return map.ContainsKey(child);
		}

		public int Count
		{
			get
			{
				return map.Count;
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<IObject> GetEnumerator()
		{
			return map.Keys.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)map.Keys).GetEnumerator();
		}

		#endregion
	}
}

