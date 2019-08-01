using System;

namespace Prawnbot.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal class NotImplementedAttribute : Attribute
    {
        public NotImplementedAttribute()
        {

        }
    }
}