using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace Portal.Controllers
{
    public class Root
    {
        protected const string Base = "api/Portal";
    }

    public class ApiController : ControllerBase
    {
    }
}
