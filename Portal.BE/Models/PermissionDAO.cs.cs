using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class PermissionDAO
    {
        public PermissionDAO()
        {
            PermissionDatas = new HashSet<PermissionDataDAO>();
            PermissionPageMappings = new HashSet<PermissionPageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }

        public virtual RoleDAO Role { get; set; }
        public virtual ICollection<PermissionDataDAO> PermissionDatas { get; set; }
        public virtual ICollection<PermissionPageMappingDAO> PermissionPageMappings { get; set; }
    }
}
