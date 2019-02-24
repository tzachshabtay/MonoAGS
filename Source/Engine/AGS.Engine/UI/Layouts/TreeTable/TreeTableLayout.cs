using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class TreeTableLayout : ITreeTableLayout
    {
        private readonly IGameEvents _events;
        private float _columnPadding;
        private float _startX;

        private bool _isDirty;
        private int _inUpdate; //For preventing re-entrancy
        private ConcurrentQueue<ITreeTableRowLayoutComponent> _newRows = new ConcurrentQueue<ITreeTableRowLayoutComponent>();

        public TreeTableLayout(IGameEvents gameEvents)
        {
            _events = gameEvents;
            ColumnSizes = new AGSBindingList<float>(10);
            Rows = new AGSBindingList<ITreeTableRowLayoutComponent>(10);
            OnRefreshLayoutNeeded = new AGSEvent<ITreeTableRowLayoutComponent>();
            OnQueryLayout = new AGSEvent<QueryLayoutEventArgs>();
            Rows.OnListChanged.Subscribe(onRowsChanged);
            gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public float ColumnPadding 
        { 
            get => _columnPadding; 
            set 
            {
                _columnPadding = value;
                OnRefreshLayoutNeeded.Invoke(null);
            } 
        }

        public float StartX
        {
            get => _startX;
            set
            {
                _startX = value;
                OnRefreshLayoutNeeded.Invoke(null);
            }
        }

        public IAGSBindingList<float> ColumnSizes { get; private set; }

        public IAGSBindingList<ITreeTableRowLayoutComponent> Rows { get; private set; }

        public IBlockingEvent<ITreeTableRowLayoutComponent> OnRefreshLayoutNeeded { get; private set; }

        public IBlockingEvent<QueryLayoutEventArgs> OnQueryLayout { get; private set; }

        public void Dispose()
        {
            Rows?.OnListChanged?.Unsubscribe(onRowsChanged);
            _events?.OnRepeatedlyExecute?.Unsubscribe(onRepeatedlyExecute);
        }

        public void PerformLayout()
        {
            _isDirty = true;
        }

        private void onRowsChanged(AGSListChangedEventArgs<ITreeTableRowLayoutComponent> args)
        {
            foreach (var row in args.Items)
            {
                _newRows.Enqueue(row.Item); 
            }
            PerformLayout();
        }

        private void onRepeatedlyExecute()
        {
            if (Interlocked.CompareExchange(ref _inUpdate, 1, 0) != 0) return;
            try
            {
                if (!_isDirty) return;
                _isDirty = false;
                performLayout();
            }
            finally
            {
                _inUpdate = 0;
            }
        }

        private void performLayout()
        {
            List<ITreeTableRowLayoutComponent> newRows = null;
            if (_newRows.Count > 0)
            {
                newRows = new List<ITreeTableRowLayoutComponent>(_newRows.Count);
                while (_newRows.TryDequeue(out var row))
                {
                    newRows.Add(row);
                }
            }
            QueryLayoutEventArgs args = new QueryLayoutEventArgs();
            OnQueryLayout.Invoke(args);
            bool isDirty = false;
            if (ColumnSizes.Count < args.ColumnSizes.Count)
            {
                isDirty = true;
                int diff = args.ColumnSizes.Count - ColumnSizes.Count;
                while (diff > 0)
                {
                    diff--;
                    ColumnSizes.Add(0f);
                }
            }
            for (int index = 0; index < args.ColumnSizes.Count; index++)
            {
                if (MathUtils.FloatEquals(ColumnSizes[index], args.ColumnSizes[index])) continue;

                isDirty = true;
                ColumnSizes[index] = args.ColumnSizes[index];
            }
            if (isDirty)
            {
                OnRefreshLayoutNeeded.Invoke(null);
            }
            else if (newRows != null)
            {
                foreach (var row in newRows)
                {
                    OnRefreshLayoutNeeded.Invoke(row); 
                }
            }
        }
    }
}
