using System;

namespace Prawnbot.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class NotImplementedAttribute : Attribute
    {
        public NotImplementedAttribute()
        {

        }
    }
}