using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class TreeLockStep
    {
        private readonly IInObjectTree _tree;
        private readonly List<ILockStep> _matrices, _boxes, _boxesWithChildren;
        private readonly List<(IImageRenderer renderer, IObject obj)> _renderers;
        private readonly Func<IObject, bool> _shouldLock;

        public TreeLockStep(IInObjectTree tree, Func<IObject, bool> shouldLock)
        {
            _tree = tree;
            _matrices = _tree == null ? new List<ILockStep>() : new List<ILockStep>(_tree.TreeNode.ChildrenCount);
            _boxes = _tree == null ? new List<ILockStep>() : new List<ILockStep>(_tree.TreeNode.ChildrenCount);
            _boxesWithChildren = _tree == null ? new List<ILockStep>() : new List<ILockStep>(_tree.TreeNode.ChildrenCount);
            _renderers = _tree == null ? new List<(IImageRenderer, IObject)>() : 
                new List<(IImageRenderer, IObject)>(_tree.TreeNode.ChildrenCount);
            _shouldLock = shouldLock;
        }

        public void Lock()
        {
            lockTree(_tree);
        }

        public void Unlock()
        {
            unlock(_matrices);
            foreach (var renderer in _renderers)
            {
                renderer.renderer.Prepare(renderer.obj, renderer.obj, AGSGame.Game.State.Viewport);
                foreach (var viewport in AGSGame.Game.State.SecondaryViewports)
                {
                    renderer.renderer.Prepare(renderer.obj, renderer.obj, viewport);
                }
            }
            unlock(_boxes);
            unlock(_boxesWithChildren);
        }

        private void unlock(List<ILockStep> locks)
        {
            foreach (var locker in locks)
            {
                locker.PrepareForUnlock();
            }
            foreach (var locker in locks)
            {
                locker.Unlock();
            }
        }

        private void lockComponent(ILockStep step, List<ILockStep> locks)
        {
            if (step == null) return;
            step.Lock();
            locks.Add(step);
        }

        private void lockTree(IInObjectTree tree)
        {
            if (tree == null) return;
            foreach (var child in tree.TreeNode.Children)
            {
                if (child == null || !_shouldLock(child)) continue;
                var matrix = child.GetComponent<IModelMatrixComponent>();
                if (matrix != null)
                {
                    lockComponent(matrix.ModelMatrixLockStep, _matrices);
                }
                var box = child.GetComponent<IBoundingBoxComponent>();
                if (box != null)
                {
                    lockComponent(box.BoundingBoxLockStep, _boxes);
                }
                var boxWithChildren = child.GetComponent<IBoundingBoxWithChildrenComponent>();
                if (boxWithChildren != null)
                {
                    lockComponent(boxWithChildren.LockStep, _boxesWithChildren);
                }
                if (child.CustomRenderer != null) _renderers.Add((child.CustomRenderer, child));
                lockTree(child);
            }
        }
    }
}
