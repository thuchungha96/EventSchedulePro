using EventSchedulePro.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X500;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;

namespace EventSchedulePro.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Username { get; set; }
            public string Staff { get; set; }
            public string MultiHTML { get; set; }
        }

        public readonly EventDBContext _context;
        public LoginModel( EventDBContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> OnPostLoginAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (String.IsNullOrEmpty(Input.Username))
            {
                ModelState.AddModelError(string.Empty, "Invalid Username");
                return Page();
            }
            var staff = _context.Staffs.FirstOrDefault(x => x.UserName == Input.Username);
            if (staff != null)
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                {
                    HttpContext.Session.SetString("UserName",staff.UserName);
                    HttpContext.Session.SetString("UserId", staff.Id.ToString());
                    HttpContext.Session.SetString("UserGroupId", staff.GroupIds ?? "");
                    HttpContext.Session.SetString("UserRole", staff.RoleUser ?? "");
                }
                return new RedirectToPageResult("/Index");
            }
            return Page();
        }
        [HttpPost("FindStaff")]
        public async Task<IActionResult> OnPostFindStaffAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            string ta = Input.MultiHTML;
            if (String.IsNullOrEmpty(Input.Staff))
            {
                ModelState.AddModelError(string.Empty, "Invalid Staff");
                return Page();
            }
            HttpContext.Session.SetString("Staff", Input.Staff);
            return new RedirectToPageResult("/Index", new { Staff = Input.Staff });
        }
    }
}
