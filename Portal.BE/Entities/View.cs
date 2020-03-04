using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class View : DataEntity,  IEquatable<View>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDeleted { get; set; }
        public List<Page> Pages { get; set; }
        public List<PermissionField> PermissionFields { get; set; }

        public bool Equals(View other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ViewFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Path { get; set; }
        public List<ViewFilter> OrFilter { get; set; }
        public ViewOrder OrderBy {get; set;}
        public ViewSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ViewOrder
    {
        Id = 1,
        Name = 2,
        Path = 3,
        IsDeleted = 4,
    }

    [Flags]
    public enum ViewSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Name = E._2,
        Path = E._3,
        IsDeleted = E._4,
    }
}
