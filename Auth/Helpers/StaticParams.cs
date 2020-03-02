using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Helpers
{
    public static class StaticParams
    {
        public static string SecretKey { get; set; }
        public static long ExpiredTime { get; set; }
        public static string AdminPassword { get; set; }
        public static string GoogleClientId { get; set; }
        public static string GoogleClientSecret { get; set; }
        public static string GoogleRedirectUri { get; set; }
    }
}
