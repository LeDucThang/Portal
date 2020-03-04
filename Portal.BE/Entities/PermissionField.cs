using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class PermissionField : DataEntity,  IEquatable<PermissionField>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long ViewId { get; set; }
        public View View { get; set; }
        public List<PermissionData> PermissionDatas { get; set; }

        public bool Equals(PermissionField other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PermissionFieldFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Type { get; set; }
        public IdFilter ViewId { get; set; }
        public List<PermissionFieldFilter> OrFilter { get; set; }
        public PermissionFieldOrder OrderBy {get; set;}
        public PermissionFieldSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionFieldOrder
    {
        Id = 1,
        Name = 2,
        Type = 3,
        View = 4,
    }

    [Flags]
    public enum PermissionFieldSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Name = E._2,
        Type = E._3,
        View = E._4,
    }
}
