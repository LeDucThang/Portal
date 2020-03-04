using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission_data
{
    public class PermissionData_PermissionDataDTO : DataDTO
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public long PermissionFieldId { get; set; }
        public string Value { get; set; }
        public PermissionData_PermissionDTO Permission { get; set; }
        public PermissionData_PermissionFieldDTO PermissionField { get; set; }

        public PermissionData_PermissionDataDTO() {}
        public PermissionData_PermissionDataDTO(PermissionData PermissionData)
        {
            this.Id = PermissionData.Id;
            this.PermissionId = PermissionData.PermissionId;
            this.PermissionFieldId = PermissionData.PermissionFieldId;
            this.Value = PermissionData.Value;
            this.Permission = PermissionData.Permission == null ? null : new PermissionData_PermissionDTO(PermissionData.Permission);
            this.PermissionField = PermissionData.PermissionField == null ? null : new PermissionData_PermissionFieldDTO(PermissionData.PermissionField);
        }
    }

    public class PermissionData_PermissionDataFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter PermissionId { get; set; }
        public IdFilter PermissionFieldId { get; set; }
        public StringFilter Value { get; set; }
        public PermissionDataOrder OrderBy { get; set; }
    }
}
