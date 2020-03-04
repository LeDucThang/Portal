using System;
using System.Collections.Generic;

namespace Portal.BE.Models
{
    public partial class PermissionDataDAO
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public long PermissionFieldId { get; set; }
        public string Value { get; set; }

        public virtual PermissionDAO Permission { get; set; }
        public virtual PermissionFieldDAO PermissionField { get; set; }
    }
}
