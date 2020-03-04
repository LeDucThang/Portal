using System;
using System.Collections.Generic;

namespace Portal.BE.Models
{
    public partial class PageDAO
    {
        public PageDAO()
        {
            PermissionPageMappings = new HashSet<PermissionPageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long ViewId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ViewDAO View { get; set; }
        public virtual ICollection<PermissionPageMappingDAO> PermissionPageMappings { get; set; }
    }
}
