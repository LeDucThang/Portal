using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class UserStatus : DataEntity,  IEquatable<UserStatus>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }

        public bool Equals(UserStatus other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class UserStatusFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<UserStatusFilter> OrFilter { get; set; }
        public UserStatusOrder OrderBy {get; set;}
        public UserStatusSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserStatusOrder
    {
        Id = 1,
        Code = 2,
        Name = 3,
    }

    [Flags]
    public enum UserStatusSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Code = E._2,
        Name = E._3,
    }
}
