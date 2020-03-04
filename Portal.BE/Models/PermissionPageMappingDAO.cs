using System;
using System.Collections.Generic;

namespace Portal.BE.Models
{
    public partial class PermissionPageMappingDAO
    {
        public long PermissionId { get; set; }
        public long PageId { get; set; }

        public virtual PageDAO Page { get; set; }
        public virtual PermissionDAO Permission { get; set; }
    }
}
