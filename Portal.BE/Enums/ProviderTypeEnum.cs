using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.BE.Enums
{
    public static class ProviderEnum
    {
        public static GenericEnum SELF => new GenericEnum { Id = 1, Code = "SELF", Name = "Self" };
        public static GenericEnum AD => new GenericEnum { Id = 2, Code = "AD", Name = "Active Directory" };
        public static GenericEnum GOOGLE = new GenericEnum { Id = 3, Code = "Google", Name = "Google" };
        public static GenericEnum Microsoft = new GenericEnum { Id = 4, Code = "Microsoft", Name = "Microsoft" };
    }
}
