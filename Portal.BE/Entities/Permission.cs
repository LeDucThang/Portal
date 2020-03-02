using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class Permission : DataEntity,  IEquatable<Permission>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
        public List<PermissionData> PermissionDatas { get; set; }
        public List<Page> Pages { get; set; }

        public bool Equals(Permission other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PermissionFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter RoleId { get; set; }
        public List<PermissionFilter> OrFilter { get; set; }
        public PermissionOrder OrderBy {get; set;}
        public PermissionSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionOrder
    {
        Id = 1,
        Name = 2,
        Role = 3,
    }

    [Flags]
    public enum PermissionSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Name = E._2,
        Role = E._3,
    }
}
