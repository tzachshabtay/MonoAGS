using System;
using System.Collections.Generic;

namespace AGS.Editor
{
    public interface ITypeDescriptor
    {
        Dictionary<InspectorCategory, List<IProperty>> GetProperties();
    }
}