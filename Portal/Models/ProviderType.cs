using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class ProviderType
    {
        public ProviderType()
        {
            Provider = new HashSet<Provider>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Provider> Provider { get; set; }
    }
}
