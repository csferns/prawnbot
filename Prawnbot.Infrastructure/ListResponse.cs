using System.Collections.Generic;

namespace Prawnbot.Infrastructure 
{
    public class ListResponse<T> : ResponseBase
    {
        public IList<T> Entities { get; set; }
    }
}
