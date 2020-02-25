using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.ViewModels
{
    public class AccountViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProviderId { get; set; }
        public string ReturnUrl { get; set; }
    }
}
