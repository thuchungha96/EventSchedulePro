using EventSchedulePro.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace EventSchedulePro.Pages.Admin
{
    public class GroupModel : PageModel
    { 
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public List<string> GroupList { get; set; }
        }

        public readonly EventDBContext _context;
        public GroupModel(EventDBContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            var userName = HttpContext.Session.GetString("AdminUserName");
            if (string.IsNullOrEmpty(userName))
            {
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
            if (Input.Username.CompareTo("Select user") != 0)
            {
                try
                {
                    var staff = _context.Staffs.FirstOrDefault(x => x.Id == int.Parse(Input.Username));
                    if (staff != null)
                    {
                        var GroupList = _context.Groups.Where(x => Input.GroupList.Contains(x.Id.ToString())).ToList();
                        if (GroupList.Any())
                        {
                            staff.GroupIds = string.Join(", ", GroupList.Select(x => x.Id.ToString()).ToArray());
                            staff.GroupNames = string.Join(", ", GroupList.Select(x => x.Name.ToString()).ToArray());
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }   
            return Page();
        }
    }
}
