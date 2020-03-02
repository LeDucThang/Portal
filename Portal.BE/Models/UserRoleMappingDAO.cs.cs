using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class UserRoleMappingDAO
    {
        public long ApplicationUserId { get; set; }
        public long RoleId { get; set; }

        public virtual ApplicationUserDAO ApplicationUser { get; set; }
        public virtual RoleDAO Role { get; set; }
    }
}
