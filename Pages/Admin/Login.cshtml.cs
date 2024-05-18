using EventSchedulePro.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace EventSchedulePro.Pages.Authen
{
    public class LoginAdminModel : PageModel
    {

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public readonly EventDBContext _context;
        public LoginAdminModel( EventDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userName = HttpContext.Session.GetString("AdminUserName");
            if (!string.IsNullOrEmpty(userName))
            {
                return new RedirectToPageResult("/Index");
            }
            return Page();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var t = Input.Username + Input.Password + Input.RememberMe;
            var user = _context.Staffs.FirstOrDefault(x => x.UserName == Input.Username && x.PasswordHash == Base64Encode(Input.Password) && x.RoleUser == "3");
            if (user != null)
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("AdminUserName")))
                {
                    HttpContext.Session.SetString("AdminUserName", user.UserName);
                    HttpContext.Session.SetString("AdminUserId", user.Id.ToString());
                    HttpContext.Session.SetString("AdminUserGroupId", user.GroupID?.ToString());
                    HttpContext.Session.SetString("AdminUserRole", user.RoleUser);
                }
                return new RedirectToPageResult("/Index");
            }
            return Page();
        }
    }
}
