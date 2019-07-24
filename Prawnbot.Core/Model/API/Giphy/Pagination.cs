using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Model.API.Giphy
{
    public class Pagination
    {
        public int total_count { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
    }
}
