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
        public string GoogleRedirectUri { get; set; }
        public string ADIP { get; set; }
        public string ADUsername { get; set; }
        public string ADPassword { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
        public string MicrosoftClientId { get; set; }
        public string MicrosoftClientSecret { get; set; }
        public string MicrosoftRedirectUri { get; set; }

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
        public StringFilter GoogleRedirectUri { get; set; }
        public StringFilter ADIP { get; set; }
        public StringFilter ADUsername { get; set; }
        public StringFilter ADPassword { get; set; }
        public StringFilter GoogleClientId { get; set; }
        public StringFilter GoogleClientSecret { get; set; }
        public StringFilter MicrosoftClientId { get; set; }
        public StringFilter MicrosoftClientSecret { get; set; }
        public StringFilter MicrosoftRedirectUri { get; set; }
        public List<ProviderFilter> OrFilter { get; set; }
        public ProviderOrder OrderBy {get; set;}
        public ProviderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderOrder
    {
        Id = 1,
        Name = 2,
        GoogleRedirectUri = 3,
        ADIP = 4,
        ADUsername = 5,
        ADPassword = 6,
        GoogleClient = 7,
        GoogleClientSecret = 8,
        MicrosoftClient = 9,
        MicrosoftClientSecret = 10,
        MicrosoftRedirectUri = 11,
    }

    [Flags]
    public enum ProviderSelect:long
    {
        ALL = E.ALL,
        Id = E._1,
        Name = E._2,
        GoogleRedirectUri = E._3,
        ADIP = E._4,
        ADUsername = E._5,
        ADPassword = E._6,
        GoogleClient = E._7,
        GoogleClientSecret = E._8,
        MicrosoftClient = E._9,
        MicrosoftClientSecret = E._10,
        MicrosoftRedirectUri = E._11,
    }
}
