using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class UserStatus
    {
        public UserStatus()
        {
            ApplicationUser = new HashSet<ApplicationUser>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
}
