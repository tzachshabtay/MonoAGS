using AGS.API;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace AGS.Engine
{    
    public static class SortDebugger
    {
        private static string _whyIsThis, _behindThis;
        private static bool _renderDebug;

        private static bool _detectCycles;

        [Conditional("DEBUG")]
        public static void RequestRenderDebug(string whyIsThis, string behindThis)
        {
            _renderDebug = true;
            _whyIsThis = whyIsThis;
            _behindThis = behindThis;
        }

        [Conditional("DEBUG")]
        public static void RequestHitTestDebug(string whyIsThis, string behindThis)
        {
            _renderDebug = false;
            _whyIsThis = whyIsThis;
            _behindThis = behindThis;
        }

        [Conditional("DEBUG")]
        public static void RequestCycleDetections()
        {
            _detectCycles = true;
        }

        [Conditional("DEBUG")]
        public static void DebugIfNeeded(List<IObject> displayList)
        {
            checkBehind(displayList);
            detectCycles(displayList);
        }

        private static void checkBehind(List<IObject> displayList)
        {
			string behindThis = _behindThis;
			if (behindThis == null) return;
			string whyIsThis = _whyIsThis;
			bool renderDebug = _renderDebug;
			_behindThis = null;

			Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
			const string sortWord = "Sort ";
			const string backwardsWord = "backwards ";
			foreach (var obj in displayList)
			{
                foreach (var property in obj.Properties.Ints.AllProperties())
				{
					if (!property.Key.Contains(sortWord)) continue;
					if (renderDebug && property.Key.Contains(backwardsWord)) continue;
					if (!renderDebug && !property.Key.Contains(backwardsWord)) continue;

					string comparedId = property.Key.Substring(renderDebug ? property.Key.IndexOf(sortWord) + sortWord.Length :
						property.Key.IndexOf(backwardsWord) + backwardsWord.Length);

                    if (property.Value > 0) map.GetOrAdd(comparedId, () => new List<string>()).Add(obj.ID);
                    else map.GetOrAdd(obj.ID, () => new List<string>()).Add(comparedId);
				}
			}
			string chain = getChain(whyIsThis, behindThis, map, new HashSet<string>());
			Debug.WriteLine(chain == null ? string.Format("{0} is not behind {1}", whyIsThis, behindThis) :
				string.Format("{0}{1}", whyIsThis, chain));
        }

        private static string getChain(string from, string to, Dictionary<string, List<string>> map, HashSet<string> checkedChain)
        {
            List<string> mapped;
            if (!map.TryGetValue(from, out mapped)) return null;
            foreach (string item in mapped)
            {
                if (item == to) return string.Format("-->{0}", to);
                if (!checkedChain.Add(item)) continue;
                string chain = getChain(item, to, map, checkedChain);
                if (chain != null) return string.Format("-->{0}{1}", item, chain);
            }
            return null;
        }

		private static void detectCycles(List<IObject> displayList)
		{
            if (!_detectCycles) return;
            _detectCycles = false;
            RenderOrderSelector comparer = new RenderOrderSelector();
			Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
			for (int i = 0; i < displayList.Count; i++)
			{
				for (int j = i + 1; j < displayList.Count; j++)
				{
					int result = comparer.Compare(displayList[i], displayList[j]);
					if (result == 0)
					{
						continue;
					}
					else if (result > 0) map.GetOrAdd(displayList[i].ID, () => new List<string>()).Add(displayList[j].ID);
					else map.GetOrAdd(displayList[j].ID, () => new List<string>()).Add(displayList[i].ID);
				}
			}
			HashSet<string> visited = new HashSet<string>();
			Dictionary<string, List<string>> recStack = new Dictionary<string, List<string>>();
			foreach (var item in displayList)
			{
				List<string> chain = findCycle(item.ID, map, visited, recStack);
				if (chain != null)
				{
					Debug.WriteLine(string.Join("=>", chain));
				}
			}
		}

		private static List<string> findCycle(string id, Dictionary<string, List<string>> map,
					   HashSet<string> visited, Dictionary<string, List<string>> recStack)
		{
			if (visited.Add(id))
			{
				List<string> items;
				if (!map.TryGetValue(id, out items)) return null;
				List<string> chain = new List<string> { id };
				recStack.Add(id, chain);
				foreach (var item in items)
				{
					List<string> cycle;
					if (!visited.Contains(item) && (cycle = findCycle(item, map, visited, recStack)) != null)
					{
						cycle = new List<string>(cycle);
						cycle.Insert(0, id);
						return cycle;
					}
					else if (recStack.TryGetValue(item, out cycle))
					{
						cycle = new List<string>(cycle);
						cycle.Insert(0, id);
						return cycle;
					}
				}
			}
			recStack.Remove(id);
			return null;
		}
    }
}
