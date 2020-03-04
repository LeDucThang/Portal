using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class PermissionFieldDAO
    {
        public PermissionFieldDAO()
        {
            PermissionDatas = new HashSet<PermissionDataDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long ViewId { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ViewDAO View { get; set; }
        public virtual ICollection<PermissionDataDAO> PermissionDatas { get; set; }
    }
}
