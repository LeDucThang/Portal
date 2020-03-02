using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class Page : DataEntity,  IEquatable<Page>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long? ParentId { get; set; }
        public List<Permission> Permissions { get; set; }

        public bool Equals(Page other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Path { get; set; }
        public IdFilter ParentId { get; set; }
        public List<PageFilter> OrFilter { get; set; }
        public PageOrder OrderBy {get; set;}
        public PageSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PageOrder
    {
        Id = 1,
        Name = 2,
        Path = 3,
        Parent = 4,
    }

    [Flags]
    public enum PageSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Name = E._2,
        Path = E._3,
        Parent = E._4,
    }
}
