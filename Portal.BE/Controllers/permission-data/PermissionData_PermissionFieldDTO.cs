using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission_data
{
    public class PermissionData_PermissionFieldDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public long ViewId { get; set; }
        

        public PermissionData_PermissionFieldDTO() {}
        public PermissionData_PermissionFieldDTO(PermissionField PermissionField)
        {
            
            this.Id = PermissionField.Id;
            
            this.Name = PermissionField.Name;
            
            this.Type = PermissionField.Type;
            
            this.ViewId = PermissionField.ViewId;
            
        }
    }

    public class PermissionData_PermissionFieldFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Type { get; set; }
        
        public IdFilter ViewId { get; set; }
        
        public PermissionFieldOrder OrderBy { get; set; }
    }
}