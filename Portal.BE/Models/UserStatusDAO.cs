using System;
using System.Collections.Generic;

namespace Portal.BE.Models
{
    public partial class UserStatusDAO
    {
        public UserStatusDAO()
        {
            ApplicationUsers = new HashSet<ApplicationUserDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUserDAO> ApplicationUsers { get; set; }
    }
}
