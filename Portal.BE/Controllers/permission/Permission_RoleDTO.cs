using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission
{
    public class Permission_RoleDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        

        public Permission_RoleDTO() {}
        public Permission_RoleDTO(Role Role)
        {
            
            this.Id = Role.Id;
            
            this.Name = Role.Name;
            
        }
    }

    public class Permission_RoleFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public RoleOrder OrderBy { get; set; }
    }
}