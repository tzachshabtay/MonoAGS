using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class TreeLockStep
    {
        private readonly IInObjectTreeComponent _tree;
        private readonly List<ILockStep> _matrices, _boxes, _boxesWithChildren, _texts;
        private readonly Func<IObject, bool> _shouldLock;

        public TreeLockStep(IInObjectTreeComponent tree, Func<IObject, bool> shouldLock)
        {
            _tree = tree;
            _matrices = _tree == null ? new List<ILockStep>() : new List<ILockStep>(_tree.TreeNode.ChildrenCount);
            _boxes = _tree == null ? new List<ILockStep>() : new List<ILockStep>(_tree.TreeNode.ChildrenCount);
            _boxesWithChildren = _tree == null ? new List<ILockStep>() : new List<ILockStep>(_tree.TreeNode.ChildrenCount);
            _texts = _tree == null ? new List<ILockStep>() : new List<ILockStep>(_tree.TreeNode.ChildrenCount);
            _shouldLock = shouldLock;
        }

        public void Lock()
        {
            lockTree(_tree);
        }

        public void Unlock()
        {
            unlock(_matrices);
            unlock(_texts);
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

        private void lockTree(IInObjectTreeComponent tree)
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
                var textComponent = child.GetComponent<ITextComponent>();
                if (textComponent != null) 
                {
                    lockComponent(textComponent.TextLockStep, _texts);
                }
                lockTree(child);
            }
        }
    }
}