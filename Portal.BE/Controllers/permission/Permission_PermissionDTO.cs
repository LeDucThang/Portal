using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.permission
{
    public class Permission_PermissionDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public Permission_RoleDTO Role { get; set; }
        public List<Permission_PermissionDataDTO> PermissionDatas { get; set; }
        public List<Permission_PageDTO> Pages { get; set; }

        public Permission_PermissionDTO() {}
        public Permission_PermissionDTO(Permission Permission)
        {
            this.Id = Permission.Id;
            this.Name = Permission.Name;
            this.RoleId = Permission.RoleId;
            this.Role = Permission.Role == null ? null : new Permission_RoleDTO(Permission.Role);
            this.PermissionDatas = Permission.PermissionDatas?.Select(x => new Permission_PermissionDataDTO(x)).ToList();
            this.Pages = Permission.Pages?.Select(x => new Permission_PageDTO(x)).ToList();
        }
    }

    public class Permission_PermissionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter RoleId { get; set; }
        public PermissionOrder OrderBy { get; set; }
    }
}
