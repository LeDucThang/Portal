using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Portal.Entities
{
    public class Provider : DataEntity,  IEquatable<Provider>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ProviderTypeId { get; set; }
        public string Value { get; set; }
        public bool IsDefault { get; set; }
        public ProviderType ProviderType { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }

        public bool Equals(Provider other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ProviderFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ProviderTypeId { get; set; }
        public StringFilter Value { get; set; }
        public List<ProviderFilter> OrFilter { get; set; }
        public ProviderOrder OrderBy {get; set;}
        public ProviderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderOrder
    {
        Id = 1,
        Name = 2,
        ProviderType = 3,
        Value = 4,
        IsDefault = 5,
    }

    [Flags]
    public enum ProviderSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Name = E._2,
        ProviderType = E._3,
        Value = E._4,
        IsDefault = E._5,
    }
}
