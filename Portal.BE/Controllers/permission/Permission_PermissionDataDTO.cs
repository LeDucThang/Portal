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
        public string FilterName { get; set; }
        public string FilterType { get; set; }
        public string FilterValue { get; set; }
        
        public Permission_PermissionDataDTO() {}
        public Permission_PermissionDataDTO(PermissionData PermissionData)
        {
            this.Id = PermissionData.Id;
            this.PermissionId = PermissionData.PermissionId;
            this.FilterName = PermissionData.FilterName;
            this.FilterType = PermissionData.FilterType;
            this.FilterValue = PermissionData.FilterValue;
        }
    }

    public class Permission_PermissionDataFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter PermissionId { get; set; }
        
        public StringFilter FilterName { get; set; }
        
        public StringFilter FilterType { get; set; }
        
        public StringFilter FilterValue { get; set; }
        
        public PermissionDataOrder OrderBy { get; set; }
    }
}