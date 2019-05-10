using System.Collections.Generic;

namespace Prawnbot.Core.Framework
{
    public class ListResponse<T> : ResponseBase
    {
        public List<T> Entities { get; set; }
    }
}
