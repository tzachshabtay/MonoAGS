using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(ITreeViewComponent))]
    public class AGSInspector : AGSComponent
    {
        private readonly Dictionary<Category, List<InspectorProperty>> _props;
        private ITreeViewComponent _treeView;
        private IUIFactory _factory;
        private IIconFactory _icons;

        public AGSInspector(IUIFactory factory, IIconFactory icons)
        {
            _props = new Dictionary<Category, List<InspectorProperty>>();
            _factory = factory;
            _icons = icons;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ITreeViewComponent>(c => { _treeView = c; configureTree(); refreshTree();}, _ => _treeView = null);
        }

        public void Show(object obj)
        {
            _props.Clear();
            var entity = obj as IEntity;
            if (entity == null)
            {
                Category cat = new Category("General");
                addProps(cat, obj);
                return;
            }
            foreach (var component in entity)
            {
                Category cat = new Category(component.Name);
                addProps(cat, component);
            }
            refreshTree();
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
			string name = prop.Name;
			if (attr != null)
			{
				if (!attr.Browsable) return null;
				if (attr.Category != null) cat = new Category(attr.Category);
				if (attr.DisplayName != null) name = attr.DisplayName;
			}
			InspectorProperty property = new InspectorProperty(obj, name, prop);
            var val = prop.GetValue(obj);
            if (val == null) return property;
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
            return property;
        }

        private PropertyInfo[] getProperties(Type type)
        {
            return type.GetRuntimeProperties().Where(p => !p.GetMethod.IsStatic && p.GetMethod.IsPublic).ToArray();

            //todo: if moving to dotnet standard 2.0, we can use this instead:
            //return type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private void configureTree()
        {
			var treeView = _treeView;
			if (treeView == null) return;
            treeView.HorizontalSpacing = 1f;
        }

        private void refreshTree()
        {
            var treeView = _treeView;
            if (treeView == null) return;
            var root = new AGSTreeStringNode { Text = ""};
            foreach (var pair in _props)
            {
                ITreeStringNode cat = addToTree(pair.Key.Name, root);
                foreach (var prop in pair.Value)
                {
                    addToTree(cat, prop);
                }
            }
            treeView.Tree = root;
            treeView.Expand(root);
        }

        private void addToTree(ITreeStringNode parent, InspectorProperty prop)
        {
            var node = addToTree(prop, parent);
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

        private ITreeStringNode addToTree(InspectorProperty property, ITreeStringNode parent)
		{
            if (!property.Prop.CanWrite)
            {
                return addToTree(string.Format("{0}: {1}", property.Name, property.Value), parent);
            }
            IInspectorPropertyEditor editor;

            var propType = property.Prop.PropertyType;
            if (propType == typeof(bool)) editor = new BoolPropertyEditor(_factory, _icons);
            else if (propType == typeof(int)) editor = new NumberPropertyEditor(_factory, true);
            else if (propType == typeof(float)) editor = new NumberPropertyEditor(_factory, false);
            else
            {
                var typeInfo = propType.GetTypeInfo();
                if (typeInfo.IsEnum)
                    editor = new EnumPropertyEditor(_factory);
                else editor = new StringPropertyEditor(_factory);
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
            public Category(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }
        }
    }
}
