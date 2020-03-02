using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class ApplicationUser : DataEntity,  IEquatable<ApplicationUser>
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
        public Provider Provider { get; set; }
        public UserStatus UserStatus { get; set; }
        public List<Role> Roles { get; set; }

        public bool Equals(ApplicationUser other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ApplicationUserFilter : FilterEntity
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
        public List<ApplicationUserFilter> OrFilter { get; set; }
        public ApplicationUserOrder OrderBy {get; set;}
        public ApplicationUserSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApplicationUserOrder
    {
        Id = 1,
        Username = 2,
        Password = 3,
        DisplayName = 4,
        Email = 5,
        Phone = 6,
        UserStatus = 7,
        RetryTime = 8,
        Provider = 9,
    }

    [Flags]
    public enum ApplicationUserSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Username = E._2,
        Password = E._3,
        DisplayName = E._4,
        Email = E._5,
        Phone = E._6,
        UserStatus = E._7,
        RetryTime = E._8,
        Provider = E._9,
    }
}
