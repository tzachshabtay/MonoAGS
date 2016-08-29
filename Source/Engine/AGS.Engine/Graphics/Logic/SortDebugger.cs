using AGS.API;
using System.Collections.Generic;
using System.Diagnostics;

namespace AGS.Engine
{    
    public static class SortDebugger
    {
        private static string _whyIsThis, _behindThis;
        private static bool _renderDebug;

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
        public static void DebugIfNeeded(List<IObject> displayList)
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
                foreach (var property in obj.AllInts())
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
    }
}
