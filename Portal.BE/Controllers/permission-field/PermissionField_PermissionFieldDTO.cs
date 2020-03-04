using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission_field
{
    public class PermissionField_PermissionFieldDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long ViewId { get; set; }
        public PermissionField_ViewDTO View { get; set; }
        public List<PermissionField_PermissionDataDTO> PermissionDatas { get; set; }

        public PermissionField_PermissionFieldDTO() {}
        public PermissionField_PermissionFieldDTO(PermissionField PermissionField)
        {
            this.Id = PermissionField.Id;
            this.Name = PermissionField.Name;
            this.Type = PermissionField.Type;
            this.ViewId = PermissionField.ViewId;
            this.View = PermissionField.View == null ? null : new PermissionField_ViewDTO(PermissionField.View);
            this.PermissionDatas = PermissionField.PermissionDatas?.Select(x => new PermissionField_PermissionDataDTO(x)).ToList();
        }
    }

    public class PermissionField_PermissionFieldFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Type { get; set; }
        public IdFilter ViewId { get; set; }
        public PermissionFieldOrder OrderBy { get; set; }
    }
}
