using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Enums
{
    public static class ProviderTypeEnum
    {
        public static GenericEnum SELF => new GenericEnum { Id = 1, Code = "SELF", Name = "Self" };
        public static GenericEnum AD => new GenericEnum { Id = 2, Code = "AD", Name = "Active Directory" };
        public static GenericEnum GOOGLE = new GenericEnum { Id = 3, Code = "Google", Name = "Google" };
        public static GenericEnum ADFS = new GenericEnum { Id = 4, Code = "ADFS", Name = "ADFS" };
    }
}
