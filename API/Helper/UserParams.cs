using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class UserParams
    {
        private const int maxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > 50 ? maxPageSize : value; 
        }

        public string Gender { get; set; }

        public int userId { get; set; }

        public int MaxAge { get; set; } = 150;

        public int MinAge { get; set; } = 18;

        public string OrderBy { get; set; } = "LastActive";



    }
}
