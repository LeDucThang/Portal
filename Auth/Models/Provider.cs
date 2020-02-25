using System;
using System.Collections.Generic;

namespace Auth.Models
{
    public partial class Provider
    {
        public Provider()
        {
            ApplicationUser = new HashSet<ApplicationUser>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long TypeId { get; set; }
        public string Value { get; set; }
        public bool IsDefault { get; set; }

        public virtual ProviderType Type { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
}
