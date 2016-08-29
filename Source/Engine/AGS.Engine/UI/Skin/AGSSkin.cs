using AGS.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
    public class AGSSkin : ISkin
    {
        private List<SkinRule> _rules = new List<SkinRule>(30);

        public const string DropDownButtonTag = "DropDownButton";
        public const string CheckBoxTag = "CheckBox";
        public const string DialogBoxTag = "DialogBox";

        public void Apply(IEntity entity)
        {
            foreach (var rule in _rules)
            {
                if (!rule.Selector(entity)) continue;
                foreach (var style in rule.Styles) style(entity);
            }
        }

        public void AddRule(Predicate<IEntity> selector, params Action<IEntity>[] styles)
        {
            _rules.Add(new SkinRule(selector, styles));
        }

        public void AddRule<TComponent>(Predicate<TComponent> selector, params Action<IEntity>[] styles) where TComponent : IComponent
        {
            Predicate<IEntity> entitySelector = entity =>
            {
                var component = entity.GetComponent<TComponent>();
                return component != null && selector(component);
            };
            AddRule(entitySelector, styles);
        }

        public void AddRule<TComponent>(Predicate<TComponent> selector, params Action<TComponent>[] styles) where TComponent : IComponent
        {
            Action<IEntity>[] entityStyles = styles.Select(s => new Action<IEntity>(entity =>
            {
               var component = entity.GetComponent<TComponent>();
               if (component != null) s(component);
            })).ToArray();
            AddRule(selector, entityStyles);
        }

        public void AddRule<TComponent>(params Action<IEntity>[] styles) where TComponent : IComponent
        {
            AddRule<TComponent>(_ => true, styles);
        }

        public void AddRule<TComponent>(params Action<TComponent>[] styles) where TComponent : IComponent
        {
            AddRule(_ => true, styles);
        }

        private class SkinRule
        {
            public SkinRule(Predicate<IEntity> selector, params Action<IEntity>[] styles)
            {
                Selector = selector;
                Styles = styles;
            }

            public Predicate<IEntity> Selector { get; private set; }
            public Action<IEntity>[] Styles { get; private set; }
        }
    }
}
