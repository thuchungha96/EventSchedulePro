using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;

namespace EventSchedulePro.Pages
{
    public class LoginModel : PageModel
    {

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            public string Username { get; set; }
            public string Staff { get; set; }
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
            using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
            {
                conn.Open();
                string sql = "SELECT * from user where Username ='" + Input.Username + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (String.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                        {
                            HttpContext.Session.SetString("UserName", rdr.GetString(1));
                            HttpContext.Session.SetString("UserId", rdr.GetInt32(0).ToString());
                            HttpContext.Session.SetString("UserGroupId", rdr.GetString(5).ToString());
                            HttpContext.Session.SetString("UserRole", rdr.GetString(3));
                        }
                        return new RedirectToPageResult("/Index");
                    }
                }
            }
            return Page();
        }
        [HttpPost("FindStaff")]
        public async Task<IActionResult> OnPostFindStaffAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (String.IsNullOrEmpty(Input.Staff))
            {
                ModelState.AddModelError(string.Empty, "Invalid Staff");
                return Page();
            }
            HttpContext.Session.SetString("Staff", Input.Staff);
            return new RedirectToPageResult("/Index", new { Staff = Input.Staff });
        }
        /*
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
            using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
            {
                conn.Open();
                string sql = "SELECT * from user where Username ='" + Input.Username + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (String.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                        {
                            HttpContext.Session.SetString("UserName", rdr.GetString(1));
                            HttpContext.Session.SetString("UserId", rdr.GetInt32(0).ToString());
                            HttpContext.Session.SetString("UserGroupId", rdr.GetInt32(2).ToString());
                            HttpContext.Session.SetString("UserRole", rdr.GetString(3));
                        }
                        return new RedirectToPageResult("/Index");
                    }
                }
            }
            return Page();
        }
        */
    }
}
