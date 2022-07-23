using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class LikeParams: PaginationParams
    {
        public string username { get; set; }
    }
}
