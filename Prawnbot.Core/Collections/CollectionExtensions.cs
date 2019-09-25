using System.Collections.Generic;

namespace Prawnbot.Core.Collections
{
    public static class CollectionExtensions
    {        
        public static Bunch<T> ToBunch<T>(this IEnumerable<T> collection)
        {
            return new Bunch<T>(collection);
        }
    }
}
