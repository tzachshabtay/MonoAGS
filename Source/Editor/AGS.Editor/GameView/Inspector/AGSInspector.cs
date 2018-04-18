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
        private readonly Dictionary<Category, List<InspectorProperty>> _props;
        private ITreeViewComponent _treeView;
        private readonly IGameFactory _factory;
        private readonly IGameSettings _settings;
        private readonly IGameState _state;
        private IEntity _currentEntity;
        private readonly ActionManager _actions;

        public AGSInspector(IGameFactory factory, IGameSettings settings, IGameState state)
        {
            _actions = new ActionManager();
            _state = state;
            _props = new Dictionary<Category, List<InspectorProperty>>();
            _factory = factory;
            _settings = settings;
        }

        public ITreeViewComponent Tree => _treeView;

        public object SelectedObject { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ITreeViewComponent>(c => { _treeView = c; configureTree(); refreshTree(); }, _ => _treeView = null);
        }

        public void Show(object obj)
        {
            SelectedObject = obj;
            _props.Clear();
            var entity = obj as IEntity;
            _currentEntity = entity;
            if (entity == null)
            {
                Category cat = new Category("General");
                addProps(cat, obj);
                refreshTree();
                return;
            }

            foreach (var component in entity)
            {
                Category cat = new Category(component.Name);
                addProps(cat, component);
            }
            addEntityProps(entity);
            refreshTree();
        }

        public void Undo() => _actions.Undo();

        public void Redo() => _actions.Redo();

        private void addEntityProps(IEntity entity)
        {
            Category cat = new Category("Hotspot");
            var prop = entity.GetType().GetProperty(nameof(IEntity.DisplayName));
            InspectorProperty property = addProp(entity, prop, ref cat);
            if (property == null) return;
            _props.GetOrAdd(cat, () => new List<InspectorProperty>()).Add(property);
        }

        private void addProps(Category defaultCategory, object obj)
        {
            var props = getProperties(obj.GetType());
            foreach (var prop in props)
            {
                var cat = defaultCategory;
                InspectorProperty property = addProp(obj, prop, ref cat);
                if (property == null) continue;
                _props.GetOrAdd(cat, () => new List<InspectorProperty>(props.Length)).Add(property);
            }
        }

        private InspectorProperty addProp(object obj, PropertyInfo prop, ref Category cat)
        {
			var attr = prop.GetCustomAttribute<PropertyAttribute>();
			if (attr == null && prop.PropertyType.FullName.StartsWith("AGS.API.IEvent", StringComparison.Ordinal)) return null; //filtering all events from the inspector by default
            if (attr == null && prop.PropertyType.FullName.StartsWith("AGS.API.IBlockingEvent", StringComparison.Ordinal)) return null; //filtering all events from the inspector by default
			string name = prop.Name;
			if (attr != null)
			{
				if (!attr.Browsable) return null;
                if (attr.Category != null) cat = new Category(attr.Category, attr.CategoryZ, attr.CategoryExpand);
				if (attr.DisplayName != null) name = attr.DisplayName;
			}
			InspectorProperty property = new InspectorProperty(obj, name, prop);
            refreshChildrenProperties(property);
            return property;
        }

        private void refreshChildrenProperties(InspectorProperty property)
        {
            property.Children.Clear();
            var val = property.Prop.GetValue(property.Object);
            if (val == null) return;
            var objType = val.GetType();
            if (objType.GetTypeInfo().GetCustomAttribute<PropertyFolderAttribute>(true) != null)
            {
                var props = getProperties(objType);
                foreach (var childProp in props)
                {
                    Category dummyCat = null;
                    var childProperty = addProp(val, childProp, ref dummyCat);
                    if (childProperty == null) continue;
                    property.Children.Add(childProperty);
                }
            }
        }

        private PropertyInfo[] getProperties(Type type)
        {
            return type.GetRuntimeProperties().Where(p => !p.GetMethod.IsStatic && p.GetMethod.IsPublic).ToArray();

            //todo: if moving to dotnet standard 2.0, we can use this instead:
            //return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private void configureTree()
        {
			var treeView = _treeView;
			if (treeView == null) return;
            treeView.HorizontalSpacing = 1f;
            treeView.VerticalSpacing = 40f;
        }

        private void refreshTree()
        {
            var treeView = _treeView;
            if (treeView == null) return;
            var root = new AGSTreeStringNode { Text = ""};
            List<ITreeStringNode> toExpand = new List<ITreeStringNode>();
            foreach (var pair in _props.OrderBy(p => p.Key.Z).ThenBy(p => p.Key.Name))
            {
                ITreeStringNode cat = addToTree(pair.Key.Name, root);
                foreach (var prop in pair.Value.OrderBy(p => p.Name))
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

        private void addToTree(ITreeStringNode parent, InspectorProperty prop)
        {
            var node = addToTree(prop, parent);
            addChildrenToTree(node, prop);
        }

        private void addChildrenToTree(ITreeStringNode node, InspectorProperty prop)
        {
            foreach (var child in prop.Children)
            {
                addToTree(node, child);
            }
        }

		private ITreeStringNode addToTree(string text, ITreeStringNode parent)
		{
            ITreeStringNode node = (ITreeStringNode)new AGSTreeStringNode { Text = text };
            return addToTree(node, parent);
		}

        private ITreeStringNode addReadonlyNodeToTree(InspectorProperty property, ITreeStringNode parent)
        {
            IInspectorPropertyEditor editor = new StringPropertyEditor(_factory, false, _actions);
            ITreeStringNode node = new InspectorTreeNode(property, editor);
            addToTree(node, parent);
            var propertyChanged = property.Object as INotifyPropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName != property.Name) return;
                    bool isExpanded = _treeView.IsCollapsed(node) == false;
                    if (isExpanded) _treeView.Collapse(node);
                    refreshChildrenProperties(property);
                    node.TreeNode.Children.Clear();
                    addChildrenToTree(node, property);

                    //todo: we'd like to enable expanding a node that was previously expanded however there's a bug that needs to be investigated before that, to reproduce:
                    //In the demo game, show the inspector for the character and expand its current room. Then move to another room.
                    //For some reason this results in endless boundin box/matrix changes until stack overflow is reached.
                    //if (isExpanded)
                      //  _treeView.Expand(node);
                };
            }
            return node;
        }

        private ITreeStringNode addToTree(InspectorProperty property, ITreeStringNode parent)
		{
            if (property.IsReadonly)
            {
                return addReadonlyNodeToTree(property, parent);
            }
            IInspectorPropertyEditor editor;

            var propType = property.Prop.PropertyType;
            if (propType == typeof(bool)) editor = new BoolPropertyEditor(_factory, _actions);
            else if (propType == typeof(Color)) editor = new ColorPropertyEditor(_factory, _actions);
            else if (propType == typeof(int)) editor = new NumberPropertyEditor(_actions, _state, _factory, true, false);
            else if (propType == typeof(float)) editor = new NumberPropertyEditor(_actions, _state, _factory, false, false);
            else if (propType == typeof(SizeF)) editor = new SizeFPropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(Size)) editor = new SizePropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(PointF)) editor = new PointFPropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(Point)) editor = new PointPropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(Vector2)) editor = new Vector2PropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(Vector3)) editor = new Vector3PropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(Vector4)) editor = new Vector4PropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(ILocation))
            {
                var entity = _currentEntity;
                var drawable = entity == null ? null : entity.GetComponent<IDrawableInfoComponent>();
                editor = new LocationPropertyEditor(_actions, _state, _factory, false, _settings, drawable);
            }
            else if (propType == typeof(RectangleF)) editor = new RectangleFPropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(Rectangle)) editor = new RectanglePropertyEditor(_actions, _state, _factory, false);
            else if (propType == typeof(int?)) editor = new NumberPropertyEditor(_actions, _state, _factory, true, true);
            else if (propType == typeof(float?)) editor = new NumberPropertyEditor(_actions, _state, _factory, false, true);
            else if (propType == typeof(SizeF?)) editor = new SizeFPropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(Size?)) editor = new SizePropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(PointF?)) editor = new PointFPropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(Point?)) editor = new PointPropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(Vector2?)) editor = new Vector2PropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(Vector3?)) editor = new Vector3PropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(Vector4?)) editor = new Vector4PropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(RectangleF?)) editor = new RectangleFPropertyEditor(_actions, _state, _factory, true);
            else if (propType == typeof(Rectangle?)) editor = new RectanglePropertyEditor(_actions, _state, _factory, true);
            else
            {
                var typeInfo = propType.GetTypeInfo();
                if (typeInfo.IsEnum)
                    editor = new EnumPropertyEditor(_factory.UI, _actions);
                else editor = new StringPropertyEditor(_factory, propType == typeof(string), _actions);
            }

            ITreeStringNode node = new InspectorTreeNode(property, editor);
            return addToTree(node, parent);
		}

        private ITreeStringNode addToTree(ITreeStringNode node, ITreeStringNode parent)
        {
			if (parent != null) node.TreeNode.SetParent(parent.TreeNode);
			return node;
        }

        private class Category
        {
            public Category(string name, int z = 0, bool expand = false)
            {
                Name = name;
                Z = z;
                Expand = expand;
            }

            public string Name { get; private set; }

            public int Z { get; private set; }

            public bool Expand { get; private set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Category);
            }

            public bool Equals(Category cat)
            {
                if (cat == null) return false;
                return Name == cat.Name;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }
    }
}