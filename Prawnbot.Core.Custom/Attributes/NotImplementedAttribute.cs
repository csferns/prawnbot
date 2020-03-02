using System;

namespace Prawnbot.Core.Custom.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class NotImplementedAttribute : Attribute
    {
        public NotImplementedAttribute()
        {

        }
    }
}