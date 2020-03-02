using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class ProviderTypeDAO
    {
        public ProviderTypeDAO()
        {
            Providers = new HashSet<ProviderDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProviderDAO> Providers { get; set; }
    }
}
