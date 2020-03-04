using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.application_user
{
    public class ApplicationUser_ApplicationUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long UserStatusId { get; set; }
        public ApplicationUser_UserStatusDTO UserStatus { get; set; }
        public List<ApplicationUser_RoleDTO> Roles { get; set; }

        public ApplicationUser_ApplicationUserDTO() {}
        public ApplicationUser_ApplicationUserDTO(ApplicationUser ApplicationUser)
        {
            this.Id = ApplicationUser.Id;
            this.Username = ApplicationUser.Username;
            this.Password = ApplicationUser.Password;
            this.DisplayName = ApplicationUser.DisplayName;
            this.Email = ApplicationUser.Email;
            this.Phone = ApplicationUser.Phone;
            this.UserStatusId = ApplicationUser.UserStatusId;
            this.UserStatus = ApplicationUser.UserStatus == null ? null : new ApplicationUser_UserStatusDTO(ApplicationUser.UserStatus);
            this.Roles = ApplicationUser.Roles?.Select(x => new ApplicationUser_RoleDTO(x)).ToList();
        }
    }

    public class ApplicationUser_ApplicationUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public IdFilter UserStatusId { get; set; }
        public ApplicationUserOrder OrderBy { get; set; }
    }
}
