using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portal.Models;

namespace Portal.Controllers
{
    [Authorize]
    public class PortalController : Controller
    {
        private readonly ILogger<PortalController> _logger;
        private readonly DataContext _context;

        public PortalController(ILogger<PortalController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Site> Sites = _context.Site.ToList();
            ViewBag.Sites = Sites;
            return View();
        }

        public IActionResult List()
        {
            if (!IsAdmin())
                return RedirectToAction("Index");
            List<Site> Sites = _context.Site.ToList();
            ViewBag.Sites = Sites;
            return View();
        }

        [HttpPost]
        public IActionResult Update([FromBody] Site site)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");
            if (site.Id == 0)
            {
                site.CreatedAt = DateTime.Now;
                site.UpdatedAt = DateTime.Now;
                site.DeletedAt = null;
                _context.Site.Add(site);
            }
            else
            {
                Site old = _context.Site.Where(s => s.Id == site.Id).FirstOrDefault();
                if (old == null)
                    return NotFound();
                old.Name = site.Name;
                old.Status = site.Status;
                old.URL = site.URL;
                old.UpdatedAt = DateTime.Now;
            }
            _context.SaveChanges();
            return View();
        }

        [HttpPost]
        public IActionResult Delete([FromBody] Site site)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");
            Site old = _context.Site.Where(s => s.Id == site.Id).FirstOrDefault();
            _context.Remove(old);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        private bool IsAdmin()
        {
            long userId;
            if (User?.Claims == null)
                userId = 0;
            userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => long.TryParse(c.Value, out long id) ? id : 0).FirstOrDefault();
            bool isAdmin = _context.UserRoleMapping
                .Any(ur => ur.ApplicationUserId == userId && ur.Role.Code == "ADMIN");
            return isAdmin;
        }
    }
}
