using System;
using System.Collections.Generic;

namespace Portal.Models
{
    public partial class ApplicationUser
    {
        public ApplicationUser()
        {
            UserRoleMapping = new HashSet<UserRoleMapping>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long StatusId { get; set; }
        public long RetryTime { get; set; }
        public long ProviderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Provider Provider { get; set; }
        public virtual UserStatus Status { get; set; }
        public virtual ICollection<UserRoleMapping> UserRoleMapping { get; set; }
    }
}
