using System.Collections.Generic;
using System.Linq;

namespace Prawnbot.Infrastructure
{
    public class ListResponse<T> : ResponseBase
    {
        public IEnumerable<T> Entities { get; set; }
        public bool HasData => Entities != null && Entities.Any();
    }
}
