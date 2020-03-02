using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class PermissionDataDAO
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public string FilterName { get; set; }
        public string FilterType { get; set; }
        public string FilterValue { get; set; }

        public virtual PermissionDAO Permission { get; set; }
    }
}
