using EventSchedulePro.Data.Class;
using EventSchedulePro.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace EventSchedulePro.Pages.Admin
{
    public class UserModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public class InputModel
        {
         
            [Required]
            public string Username { get; set; }

            [DataType(DataType.Password)]
            public string Password { get; set; }
       
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public readonly EventDBContext _context;
        public UserModel(EventDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {

            var userName = HttpContext.Session.GetString("AdminUserName");
            if (string.IsNullOrEmpty(userName)){
                return new RedirectToPageResult("/Admin/Login");
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
            ErrorMessage = "";
            if (!string.IsNullOrEmpty(Input.Username))
            {
                var staff = _context.Staffs.FirstOrDefault(x => x.UserName == Input.Username);
                // staff != null then that staff aready exited in database.
                if (staff == null)
                {

                    Staff newStaff = new Staff { UserName = Input.Username, GroupID = 0, RoleUser = "1", PasswordHash = Base64Encode("1") };
                    _context.Add(newStaff);
                    await _context.SaveChangesAsync();
                } else
                {
                    ErrorMessage = $"[{Input.Username}] Already Exited!";
                }
            }
           
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            return Page();
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> OnPostDeleteAsync(string Id = null)
        {
            if (Id != null)
            {             
                try
                {
                    var staff = _context.Staffs.FirstOrDefault(x => x.Id == int.Parse(Id));
                    if (staff != null)
                    {
                        _context.Remove(staff);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception ex) { }
            }
            return Page();
        }
    }
}
