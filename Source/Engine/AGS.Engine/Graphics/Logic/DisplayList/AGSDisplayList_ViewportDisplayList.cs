using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSDisplayList
    {
        private class ViewportDisplayList
        {
            private LayerListComparer _comparer;

            public ViewportDisplayList()
            {
                _comparer = new LayerListComparer();
                DisplayListPerLayer = new ConcurrentDictionary<int, LayerDisplayList>();
            }

            public List<IObject> DisplayList { get; private set; }
            public ConcurrentDictionary<int, LayerDisplayList> DisplayListPerLayer { get; private set; }

            public void Sort()
            {
                List<(int z, List<IObject> items)> layers = new List<(int, List<IObject>)>();
                int count = 0;
                foreach (var pair in DisplayListPerLayer)
                {
                    count += pair.Value.Items.Count;
                    pair.Value.Sort();
                    layers.Add((pair.Key, pair.Value.Items));
                }
                layers.Sort(_comparer);
                var displayList = new List<IObject>(count);
                foreach (var layer in layers)
                {
                    displayList.AddRange(layer.items);
                }
                DisplayList = displayList;
            }
        }

        private class LayerListComparer : IComparer<(int z, List<IObject>)>
        {
            public int Compare((int z, List<IObject>) x, (int z, List<IObject>) y)
            {
                return y.z - x.z;
            }
        }

        private class LayerDisplayList
        {
            private IComparer<IObject> _comparer;
            private bool _isDirty;

            public LayerDisplayList(IComparer<IObject> comparer)
            {
                _comparer = comparer;
                IsDirty = true;
            }

            public bool IsDirty
            {
                get => _isDirty;
                set
                {
                    _isDirty = value;
                    if (value) Items = new List<IObject>(100);
                }
            }
            public List<IObject> Items { get; private set; }

            public void Sort()
            {
                if (!IsDirty) return;
                Items.Sort(_comparer);
                IsDirty = false;
            }
        }
    }
}