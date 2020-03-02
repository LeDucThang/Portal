using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Portal.BE.Controllers
{
    public class MyAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => long.TryParse(c.Value, out long id) ? id : 0)
                .FirstOrDefault();
            if (userId == 0)
                context.Result = new ForbidResult();
        }
    }
}
