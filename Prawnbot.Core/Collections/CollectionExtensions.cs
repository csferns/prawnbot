using System;
using System.Collections.Generic;
using System.Linq;

namespace Prawnbot.Core.Collections
{
    public static class CollectionExtensions
    {        
        public static T RandomOrDefault<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return default(T);
            }

            Random random = new Random();
            int index = random.Next(0, collection.Count());

            return collection.ElementAtOrDefault(index);
        }
    }
}
