using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Portal.Entities;

namespace Portal.Controllers.provider
{
    public class Provider_ProviderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ProviderTypeId { get; set; }
        public string Value { get; set; }
        public bool IsDefault { get; set; }
        public Provider_ProviderTypeDTO ProviderType { get; set; }
        public List<Provider_ApplicationUserDTO> ApplicationUsers { get; set; }

        public Provider_ProviderDTO() {}
        public Provider_ProviderDTO(Provider Provider)
        {
            this.Id = Provider.Id;
            this.Name = Provider.Name;
            this.ProviderTypeId = Provider.ProviderTypeId;
            this.Value = Provider.Value;
            this.IsDefault = Provider.IsDefault;
            this.ProviderType = Provider.ProviderType == null ? null : new Provider_ProviderTypeDTO(Provider.ProviderType);
            this.ApplicationUsers = Provider.ApplicationUsers?.Select(x => new Provider_ApplicationUserDTO(x)).ToList();
        }
    }

    public class Provider_ProviderFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ProviderTypeId { get; set; }
        public StringFilter Value { get; set; }
        public ProviderOrder OrderBy { get; set; }
    }
}
