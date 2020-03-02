using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class ProviderDAO
    {
        public ProviderDAO()
        {
            ApplicationUsers = new HashSet<ApplicationUserDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long ProviderTypeId { get; set; }
        public string Value { get; set; }
        public bool IsDefault { get; set; }

        public virtual ProviderTypeDAO ProviderType { get; set; }
        public virtual ICollection<ApplicationUserDAO> ApplicationUsers { get; set; }
    }
}
