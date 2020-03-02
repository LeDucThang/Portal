using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.role
{
    public class Role_ApplicationUserDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }
        
        public long UserStatusId { get; set; }
        
        public long RetryTime { get; set; }
        
        public long ProviderId { get; set; }
        

        public Role_ApplicationUserDTO() {}
        public Role_ApplicationUserDTO(ApplicationUser ApplicationUser)
        {
            
            this.Id = ApplicationUser.Id;
            
            this.Username = ApplicationUser.Username;
            
            this.Password = ApplicationUser.Password;
            
            this.DisplayName = ApplicationUser.DisplayName;
            
            this.Email = ApplicationUser.Email;
            
            this.Phone = ApplicationUser.Phone;
            
            this.UserStatusId = ApplicationUser.UserStatusId;
            
            this.RetryTime = ApplicationUser.RetryTime;
            
            this.ProviderId = ApplicationUser.ProviderId;
            
        }
    }

    public class Role_ApplicationUserFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Username { get; set; }
        
        public StringFilter Password { get; set; }
        
        public StringFilter DisplayName { get; set; }
        
        public StringFilter Email { get; set; }
        
        public StringFilter Phone { get; set; }
        
        public IdFilter UserStatusId { get; set; }
        
        public LongFilter RetryTime { get; set; }
        
        public IdFilter ProviderId { get; set; }
        
        public ApplicationUserOrder OrderBy { get; set; }
    }
}