using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class Role : DataEntity,  IEquatable<Role>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }

        public bool Equals(Role other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class RoleFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public List<RoleFilter> OrFilter { get; set; }
        public RoleOrder OrderBy {get; set;}
        public RoleSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoleOrder
    {
        Id = 1,
        Name = 2,
    }

    [Flags]
    public enum RoleSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Name = E._2,
    }
}
