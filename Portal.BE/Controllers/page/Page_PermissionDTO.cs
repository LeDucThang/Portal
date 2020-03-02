using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.page
{
    public class Page_PermissionDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long RoleId { get; set; }
        

        public Page_PermissionDTO() {}
        public Page_PermissionDTO(Permission Permission)
        {
            
            this.Id = Permission.Id;
            
            this.Name = Permission.Name;
            
            this.RoleId = Permission.RoleId;
            
        }
    }

    public class Page_PermissionFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter RoleId { get; set; }
        
        public PermissionOrder OrderBy { get; set; }
    }
}