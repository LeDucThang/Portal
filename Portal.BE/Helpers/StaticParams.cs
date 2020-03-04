using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers
{
    public class StaticParams
    {
        public static DateTime DateTimeNow => DateTimeNow;
        public static DateTime DateTimeMin => DateTime.MinValue;
        public static string SecretKey { get; set; }
        public static string AdminPassword { get; set; } = string.Empty;
    }
}
