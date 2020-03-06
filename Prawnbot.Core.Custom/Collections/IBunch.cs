using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Custom.Collections
{
    public interface IBunch<T> : IList<T>, IEnumerable<T>, ICollection<T>
    {
        T Random();
        T Random(Func<T, bool> predicate);
        T RandomOrDefault();
        T RandomOrDefault(Func<T, bool> predicate);
        void AddRange(IBunch<T> collection);
        IBunch<T> RemoveAll(Predicate<T> predicate);
    }
}
