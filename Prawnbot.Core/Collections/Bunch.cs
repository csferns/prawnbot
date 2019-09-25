using System;
using System.Collections.Generic;
using System.Linq;

namespace Prawnbot.Core.Collections
{
    public class Bunch<T> : List<T>, IList<T>
    {
        public Bunch()
        {

        }

        public Bunch(int capacity)
        {
            this.Capacity = capacity;
        }

        public Bunch(IEnumerable<T> collection)
        {
            this.AddRange(collection);
        }

        public T Random() 
        {
            return Random(this);
        }

        public T RandomOrDefault()
        {
            return this.Any() && this != null 
                ? Random(this) 
                : default(T);
        }

        public T Random(Func<T, bool> predicate)
        {
            return Random(this.Where(predicate));
        }

        public T RandomOrDefault(Func<T, bool> predicate)
        {
            return this.Any() && this != null
                ? Random(this.Where(predicate))
                : default(T);
        }

        private T Random(IEnumerable<T> collection)
        {
            Random random = new Random();
            Bunch<T> newList = new Bunch<T>();

            foreach (T item in collection)
            {
                newList.AddRange(Enumerable.Repeat(item, 3));
            }

            return newList[random.Next(this.Count())];
        }
    }
}
