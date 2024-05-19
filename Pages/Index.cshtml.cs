using EventSchedulePro.Data;
using EventSchedulePro.Data.Class;
using EventSchedulePro.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
namespace EventSchedulePro.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public readonly EventDBContext _context;
        public IndexModel(ILogger<IndexModel> logger, EventDBContext context)
        {
            _logger = logger;
            _context = context;
        }
        public ActionResult Index(string Staff)
        {         

            return Page();

        }
        public IActionResult OnGet()
        {

            var userNameAdmin = HttpContext.Session.GetString("AdminUserName");
            var userName = HttpContext.Session.GetString("UserName");
            var staff = HttpContext.Request.Query["Staff"];
            if (!string.IsNullOrEmpty(staff))
            {
                HttpContext.Session.SetString("Staff", staff);
            }
            staff = HttpContext.Session.GetString("Staff");
            if (string.IsNullOrEmpty(userNameAdmin) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(staff))
            {
                return new RedirectToPageResult("/Login");
            }
            return Page();
        }
    }
}
