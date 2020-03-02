using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.application_user
{
    public class ApplicationUser_RoleDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        

        public ApplicationUser_RoleDTO() {}
        public ApplicationUser_RoleDTO(Role Role)
        {
            
            this.Id = Role.Id;
            
            this.Name = Role.Name;
            
        }
    }

    public class ApplicationUser_RoleFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public RoleOrder OrderBy { get; set; }
    }
}