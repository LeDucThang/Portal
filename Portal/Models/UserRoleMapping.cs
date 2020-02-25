using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class UserRoleMapping
    {
        public long ApplicationUserId { get; set; }
        public long RoleId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Role Role { get; set; }
    }
}
