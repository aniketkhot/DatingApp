using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class UserParams : PaginationParams
    {
        public string Gender { get; set; }

        public int userId { get; set; }

        public int MaxAge { get; set; } = 150;

        public int MinAge { get; set; } = 18;

        public string OrderBy { get; set; } = "LastActive";



    }
}
