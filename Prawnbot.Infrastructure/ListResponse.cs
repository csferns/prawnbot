using System.Collections.Generic;
using System.Linq;

namespace Prawnbot.Infrastructure
{
    public class ListResponse<T> : ResponseBase
    {
        public IList<T> Entities { get; set; }
        public bool HasData => Entities.Any() && Entities != null;
    }
}
