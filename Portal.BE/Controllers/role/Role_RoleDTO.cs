using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.role
{
    public class Role_RoleDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Role_PermissionDTO> Permissions { get; set; }
        public List<Role_ApplicationUserDTO> ApplicationUsers { get; set; }

        public Role_RoleDTO() {}
        public Role_RoleDTO(Role Role)
        {
            this.Id = Role.Id;
            this.Name = Role.Name;
            this.Permissions = Role.Permissions?.Select(x => new Role_PermissionDTO(x)).ToList();
            this.ApplicationUsers = Role.ApplicationUsers?.Select(x => new Role_ApplicationUserDTO(x)).ToList();
        }
    }

    public class Role_RoleFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public RoleOrder OrderBy { get; set; }
    }
}
