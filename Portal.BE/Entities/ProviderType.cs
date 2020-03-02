using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class ProviderType : DataEntity,  IEquatable<ProviderType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Provider> Providers { get; set; }

        public bool Equals(ProviderType other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ProviderTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<ProviderTypeFilter> OrFilter { get; set; }
        public ProviderTypeOrder OrderBy {get; set;}
        public ProviderTypeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderTypeOrder
    {
        Id = 1,
        Code = 2,
        Name = 3,
    }

    [Flags]
    public enum ProviderTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Code = E._2,
        Name = E._3,
    }
}
