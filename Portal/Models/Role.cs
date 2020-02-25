using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class Role
    {
        public Role()
        {
            UserRoleMapping = new HashSet<UserRoleMapping>();
        }

        public long Id { get; set; }
        public string Code { get; set; }

        public virtual ICollection<UserRoleMapping> UserRoleMapping { get; set; }
    }
}
