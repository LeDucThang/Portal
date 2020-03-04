using System;
using System.Collections.Generic;

namespace Portal.BE.Models
{
    public partial class RoleDAO
    {
        public RoleDAO()
        {
            Permissions = new HashSet<PermissionDAO>();
            UserRoleMappings = new HashSet<UserRoleMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PermissionDAO> Permissions { get; set; }
        public virtual ICollection<UserRoleMappingDAO> UserRoleMappings { get; set; }
    }
}
