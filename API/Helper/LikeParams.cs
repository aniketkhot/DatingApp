using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class LikeParams: PaginationParams
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }
}
