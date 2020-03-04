using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission_field
{
    public class PermissionField_PermissionDataDTO : DataDTO
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public long PermissionFieldId { get; set; }
        public string Value { get; set; }
        
        public PermissionField_PermissionDataDTO() {}
        public PermissionField_PermissionDataDTO(PermissionData PermissionData)
        {
            this.Id = PermissionData.Id;
            this.PermissionId = PermissionData.PermissionId;
            this.PermissionFieldId = PermissionData.PermissionFieldId;
            this.Value = PermissionData.Value;
        }
    }

    public class PermissionField_PermissionDataFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter PermissionId { get; set; }
        
        public IdFilter PermissionFieldId { get; set; }
        
        public StringFilter Value { get; set; }
        
        public PermissionDataOrder OrderBy { get; set; }
    }
}