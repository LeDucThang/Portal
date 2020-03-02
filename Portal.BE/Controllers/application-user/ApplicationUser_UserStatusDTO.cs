using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.application_user
{
    public class ApplicationUser_UserStatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public ApplicationUser_UserStatusDTO() {}
        public ApplicationUser_UserStatusDTO(UserStatus UserStatus)
        {
            
            this.Id = UserStatus.Id;
            
            this.Code = UserStatus.Code;
            
            this.Name = UserStatus.Name;
            
        }
    }

    public class ApplicationUser_UserStatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public UserStatusOrder OrderBy { get; set; }
    }
}