using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class PermissionData : DataEntity,  IEquatable<PermissionData>
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public long PermissionFieldId { get; set; }
        public string Value { get; set; }
        public Permission Permission { get; set; }
        public PermissionField PermissionField { get; set; }

        public bool Equals(PermissionData other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PermissionDataFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter PermissionId { get; set; }
        public IdFilter PermissionFieldId { get; set; }
        public StringFilter Value { get; set; }
        public List<PermissionDataFilter> OrFilter { get; set; }
        public PermissionDataOrder OrderBy {get; set;}
        public PermissionDataSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionDataOrder
    {
        Id = 1,
        Permission = 2,
        PermissionField = 3,
        Value = 4,
    }

    [Flags]
    public enum PermissionDataSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Permission = E._2,
        PermissionField = E._3,
        Value = E._4,
    }
}
