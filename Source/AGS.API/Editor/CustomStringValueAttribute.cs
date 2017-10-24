using System;
namespace AGS.API
{
    public enum CustomStringApplyWhen
    {
        ReadOnly,
        CanWrite,
        Both
    }

    /// <summary>
    /// An attribute to be placed on a method that will be used instead of ToString() to supply the string display value
    /// to the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomStringValueAttribute : Attribute
    {
        public CustomStringValueAttribute(CustomStringApplyWhen applyWhen)
        {
            ApplyWhen = applyWhen;    
        }

        public CustomStringApplyWhen ApplyWhen { get; private set; }
    }
}
