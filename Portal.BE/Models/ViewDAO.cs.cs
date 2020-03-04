using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class ViewDAO
    {
        public ViewDAO()
        {
            Pages = new HashSet<PageDAO>();
            PermissionFields = new HashSet<PermissionFieldDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<PageDAO> Pages { get; set; }
        public virtual ICollection<PermissionFieldDAO> PermissionFields { get; set; }
    }
}
