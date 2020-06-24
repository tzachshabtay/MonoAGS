using System;
namespace AGS.API
{
    /// <summary>
    /// Allows declaring a class as a folder of properties which can be expanded in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PropertyFolderAttribute : Attribute
    {
    }
}
