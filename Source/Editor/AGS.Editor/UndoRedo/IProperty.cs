using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Editor
{
    public interface IProperty
    {
        string Name { get; }
        string DisplayName { get; }
        object Object { get; }
        IComponent Component { get; }
        IProperty Parent { get; }
        ValueModel Value { get; set; }
        string ValueString { get; }
        Type PropertyType { get; }
        TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute;
        List<IProperty> Children { get; }
        bool IsReadonly { get; }
        void Refresh();
    }
}
