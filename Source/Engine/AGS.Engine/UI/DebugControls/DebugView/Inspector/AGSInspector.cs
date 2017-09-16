using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(ITreeViewComponent))]
    public class AGSInspector : AGSComponent
    {
        private readonly Dictionary<Category, List<Property>> _props;
        private ITreeViewComponent _treeView;

        public AGSInspector()
        {
            _props = new Dictionary<Category, List<Property>>();
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
            var props = obj.GetType().GetRuntimeProperties().ToList();
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<PropertyAttribute>();
                if (attr == null && prop.PropertyType.FullName.StartsWith("AGS.API.IEvent", StringComparison.Ordinal)) continue; //filtering all events from the inspector by default
                Category cat = defaultCategory;
                string name = prop.Name;
                if (attr != null)
                {
                    if (!attr.Browsable) continue;
                    if (attr.Category != null) cat = new Category(attr.Category);
                    if (attr.DisplayName != null) name = attr.DisplayName;
                }
                Property property = new Property(obj, name, prop);
                _props.GetOrAdd(cat, () => new List<Property>(props.Count)).Add(property);
            }
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
            var root = new AGSTreeStringNode();
            foreach (var pair in _props)
            {
                ITreeStringNode cat = addToTree(pair.Key.Name, root);
                foreach (var prop in pair.Value)
                {
                    addToTree(string.Format("{0}: {1}", prop.Name, prop.Value), cat);
                }
            }
            treeView.Tree = root;
        }

		private ITreeStringNode addToTree(string text, ITreeStringNode parent)
		{
			var node = new AGSTreeStringNode { Text = text };
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

        private class Property
        {
            public Property(object obj, string name, PropertyInfo prop)
            {
                Prop = prop;
                Name = name;
                Object = obj;
                Refresh();
            }

            public string Name { get; private set; }
            public string Value { get; private set; }
            public PropertyInfo Prop { get; private set; }
            public object Object { get; private set; }

            public void Refresh()
            {
				object val = Prop.GetValue(Object);
				Value = val == null ? "(null)" : val.ToString();
            }
        }
    }
}
