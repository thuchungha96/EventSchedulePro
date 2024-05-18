using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventSchedulePro.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserGroupId");
            HttpContext.Session.Remove("UserRole");

            HttpContext.Session.Remove("AdminUserName");
            HttpContext.Session.Remove("AdminUserId");
            HttpContext.Session.Remove("AdminUserGroupId");
            HttpContext.Session.Remove("AdminUserRole");
            HttpContext.Session.Remove("Staff");
            return new RedirectToPageResult("/Index");
        }
    }
}
