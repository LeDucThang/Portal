using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.provider
{
    public class Provider_ProviderTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Provider_ProviderTypeDTO() {}
        public Provider_ProviderTypeDTO(ProviderType ProviderType)
        {
            
            this.Id = ProviderType.Id;
            
            this.Code = ProviderType.Code;
            
            this.Name = ProviderType.Name;
            
        }
    }

    public class Provider_ProviderTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ProviderTypeOrder OrderBy { get; set; }
    }
}