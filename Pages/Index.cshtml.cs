using EventSchedulePro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
namespace EventSchedulePro.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
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
            if (string.IsNullOrEmpty(userNameAdmin) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(staff))
            {
                return new RedirectToPageResult("/Login");
            }
            return Page();
        }
    }
}
