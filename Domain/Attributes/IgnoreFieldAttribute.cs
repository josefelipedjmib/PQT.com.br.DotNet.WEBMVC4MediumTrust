using System;

namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreFieldAttribute : Attribute
    {
    }
}
