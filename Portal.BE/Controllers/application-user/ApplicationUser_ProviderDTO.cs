using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.application_user
{
    public class ApplicationUser_ProviderDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long ProviderTypeId { get; set; }
        
        public string Value { get; set; }
        
        public bool IsDefault { get; set; }
        

        public ApplicationUser_ProviderDTO() {}
        public ApplicationUser_ProviderDTO(Provider Provider)
        {
            
            this.Id = Provider.Id;
            
            this.Name = Provider.Name;
            
            this.ProviderTypeId = Provider.ProviderTypeId;
            
            this.Value = Provider.Value;
            
            this.IsDefault = Provider.IsDefault;
            
        }
    }

    public class ApplicationUser_ProviderFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter ProviderTypeId { get; set; }
        
        public StringFilter Value { get; set; }
        
        public ProviderOrder OrderBy { get; set; }
    }
}