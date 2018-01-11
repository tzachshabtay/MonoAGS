using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSMatrixUpdater : IMatrixUpdater
    {
        private readonly Stack<IObject> _parentStack;
        private readonly HashSet<string> _alreadyRefreshedIdsCache;

        public AGSMatrixUpdater()
        {
            _parentStack = new Stack<IObject>();
            _alreadyRefreshedIdsCache = new HashSet<string>();
        }

        public void ClearCache()
        {
            _alreadyRefreshedIdsCache.Clear();
        }

        public void RefreshMatrix(IObject obj)
        {
            //Making sure all of the parents have their matrix refreshed before rendering the object,
            //as if they need a new matrix the object will need to recalculate its matrix as well.
            var parent = obj;
            while (parent != null)
            {
                if (_alreadyRefreshedIdsCache.Contains(parent.ID)) break;
                _parentStack.Push(parent);
                parent = parent.TreeNode.Parent;
            }
            while (_parentStack.Count > 0)
            {
                parent = _parentStack.Pop();
                _alreadyRefreshedIdsCache.Add(parent.ID);
                parent.GetModelMatrices();
            }
        }
    }
}
