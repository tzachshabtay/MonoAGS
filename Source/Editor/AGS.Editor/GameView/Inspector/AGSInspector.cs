using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    [RequiredComponent(typeof(ITreeViewComponent))]
    public class AGSInspector : AGSComponent, IInspectorComponent
    {
        private Dictionary<InspectorCategory, List<IProperty>> _props;
        private ITreeViewComponent _treeView;
        private readonly IGameFactory _factory;
        private readonly IFont _font;
        private readonly IGameState _state;
        private readonly StateModel _model;
        private IEntity _currentEntity;
        private readonly ActionManager _actions;
        private List<Action> _cleanup;
        private readonly EditorProvider _editorProvider;

        public AGSInspector(IGameFactory factory, IGameSettings gameSettings, IGameSettings editorSettings, IGameState state, 
                            ActionManager actions, StateModel model, AGSEditor editor, IObject parentDialog)
        {
            _cleanup = new List<Action>(50);
            _actions = actions;
            _model = model;
            _state = state;
            _props = new Dictionary<InspectorCategory, List<IProperty>>();
            _factory = factory;
            _font = editorSettings.Defaults.TextFont;
            _editorProvider = new EditorProvider(factory, actions, model, state, gameSettings, editor, parentDialog);
        }

        public ITreeViewComponent Tree => _treeView;

        public object SelectedObject { get; private set; }

        public bool SortValues { get; set; } = true;

        public Dictionary<InspectorCategory, List<IProperty>> Properties => _props;

        public override void Init()
        {
            base.Init();
            Entity.Bind<ITreeViewComponent>(c => { _treeView = c; configureTree(); refreshTree(); }, _ => _treeView = null);
        }

        public void Show(object obj)
        {
            cleanup();
            SelectedObject = obj;
            _props.Clear();
            GC.Collect(2, GCCollectionMode.Forced);
            var descriptor = getTypeDescriptor(obj);
            _props = descriptor.GetProperties();
            refreshTree();
        }

        public void Undo() => _actions.Undo();

        public void Redo() => _actions.Redo();

        private ITypeDescriptor getTypeDescriptor(object obj)
        {
            _currentEntity = null;
            if (obj is ITypeDescriptor descriptor) return descriptor;
            if (obj is IEntity entity)
            {
                _currentEntity = entity;
                return new EntityTypeDescriptor(entity);
            }
            return new ObjectTypeDescriptor(obj);
        }

        private void configureTree()
        {
			var treeView = _treeView;
			if (treeView == null) return;
            treeView.VerticalSpacing = 40f;
        }

        private void refreshTree()
        {
            var treeView = _treeView;
            if (treeView == null) return;
            var root = new AGSTreeStringNode("", _font);
            List<ITreeStringNode> toExpand = new List<ITreeStringNode>();
            bool skipCategories = _props.Count == 1;
            foreach (var pair in _props.OrderBy(p => p.Key.Z).ThenBy(p => p.Key.Name))
            {
                ITreeStringNode cat = skipCategories ? root : addToTree(pair.Key.Name, root);
                IEnumerable<IProperty> values = SortValues ? pair.Value.OrderBy(p => p.Name) : (IEnumerable<IProperty>)pair.Value;
                foreach (var prop in values)
                {
                    addToTree(cat, prop);
                }
                if (pair.Key.Expand)
                {
                    toExpand.Add(cat);
                }
            }
            treeView.Tree = root;
            treeView.Expand(root);
            foreach (var node in toExpand)
            {
                treeView.Expand(node);
            }
        }

        private void addToTree(ITreeStringNode parent, IProperty prop)
        {
            var node = addToTree(prop, parent);
            addChildrenToTree(node, prop);
        }

        private void addChildrenToTree(ITreeStringNode node, IProperty prop)
        {
            foreach (var child in prop.Children)
            {
                addToTree(node, child);
            }
        }

		private ITreeStringNode addToTree(string text, ITreeStringNode parent)
		{
            ITreeStringNode node = new AGSTreeStringNode(text, _font);
            return addToTree(node, parent);
		}

        private ITreeStringNode addReadonlyNodeToTree(IProperty property, ITreeStringNode parent)
        {
            IInspectorPropertyEditor editor = new StringPropertyEditor(_factory, false, _actions, _model);
            ITreeStringNode node = new InspectorTreeNode(property, editor, _font);
            addToTree(node, parent);
            if (property.Object is INotifyPropertyChanged propertyChanged)
            {
                PropertyChangedEventHandler onPropertyChanged = (sender, e) =>
                {
                    if (e.PropertyName != property.Name) return;
                    bool isExpanded = _treeView.IsCollapsed(node) == false;
                    if (isExpanded) _treeView.Collapse(node);
                    ObjectTypeDescriptor.RefreshChildrenProperties(property);
                    node.TreeNode.Children.Clear();
                    addChildrenToTree(node, property);

                    //todo: we'd like to enable expanding a node that was previously expanded however there's a bug that needs to be investigated before that, to reproduce:
                    //In the demo game, show the inspector for the character and expand its current room. Then move to another room.
                    //For some reason this results in endless boundin box/matrix changes until stack overflow is reached.
                    //if (isExpanded)
                      //  _treeView.Expand(node);
                };
                propertyChanged.PropertyChanged += onPropertyChanged;
                _cleanup.Add(() => propertyChanged.PropertyChanged -= onPropertyChanged);
            }
            return node;
        }

        private void cleanup()
        {
            foreach (var clean in _cleanup) clean();
            _cleanup.Clear();
        }

        private ITreeStringNode addToTree(IProperty property, ITreeStringNode parent)
		{
            if (property.IsReadonly)
            {
                return addReadonlyNodeToTree(property, parent);
            }

            var propType = property.PropertyType;
            var editor = _editorProvider.GetEditor(propType, _currentEntity);

            ITreeStringNode node = new InspectorTreeNode(property, editor, _font);
            return addToTree(node, parent);
		}

        private ITreeStringNode addToTree(ITreeStringNode node, ITreeStringNode parent)
        {
			if (parent != null) node.TreeNode.SetParent(parent.TreeNode);
			return node;
        }
    }
}
