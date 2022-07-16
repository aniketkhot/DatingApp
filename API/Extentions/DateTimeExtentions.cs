using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extentions
{
    public static class DateTimeExtentions
    {
        public static int CalculateAge(this DateTime DateOfBirth)
        {
            return 1;

            int age = DateTime.Now.Year - DateOfBirth.Year;

            if (DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;

            return age;
        }
    }
}
