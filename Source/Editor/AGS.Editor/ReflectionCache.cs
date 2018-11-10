using System;
using System.Collections.Generic;
using System.Linq;
using AGS.Engine;

namespace AGS.Editor
{
    public static class ReflectionCache
    {
        private static Dictionary<Type, List<Type>> _implementors;

        public static List<Type> AllTypes { get; private set; }

        public static List<Type> AllImplementations(this Type baseType)
        {
            return _implementors.GetOrAdd(baseType, type => AllTypes.Where(p => baseType.IsAssignableFrom(p)).ToList());
        }

        public static void Refresh()
        {
            AllTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).ToList();
            _implementors = new Dictionary<Type, List<Type>>();
        }
    }
}