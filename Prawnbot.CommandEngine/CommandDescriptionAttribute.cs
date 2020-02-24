using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.CommandEngine
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
        // This is a positional argument
        public CommandAttribute()
        {
        }

        public string Description { get; set; }

        public string[] Parameters { get; set; }

        public string[] OptionalParameters { get; set; }

        public string CommandText { get; set; }
    }
}
