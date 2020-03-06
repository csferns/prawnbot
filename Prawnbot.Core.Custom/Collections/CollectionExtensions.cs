using System;
using System.Collections.Generic;

namespace Prawnbot.Core.Custom.Collections
{
    public static class CollectionExtensions
    {        
        public static Bunch<T> ToBunch<T>(this IEnumerable<T> collection)
        {
            return new Bunch<T>(collection);
        }

        public static IBunch<T> RemoveAll<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            Bunch<T> bunch = new Bunch<T>(collection);

            bunch.RemoveAll(predicate);

            return bunch;
        }
    }
}
