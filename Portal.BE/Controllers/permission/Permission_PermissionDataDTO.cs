using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission
{
    public class Permission_PermissionDataDTO : DataDTO
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public long PermissionFieldId { get; set; }
        public string Value { get; set; }
        
        public Permission_PermissionDataDTO() {}
        public Permission_PermissionDataDTO(PermissionData PermissionData)
        {
            this.Id = PermissionData.Id;
            this.PermissionId = PermissionData.PermissionId;
            this.PermissionFieldId = PermissionData.PermissionFieldId;
            this.Value = PermissionData.Value;
        }
    }

    public class Permission_PermissionDataFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter PermissionId { get; set; }
        
        public IdFilter PermissionFieldId { get; set; }
        
        public StringFilter Value { get; set; }
        
        public PermissionDataOrder OrderBy { get; set; }
    }
}